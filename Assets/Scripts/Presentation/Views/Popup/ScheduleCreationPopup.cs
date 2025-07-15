using System;
using System.Collections.Generic;
using System.Globalization;
using AppCore.UseCases;
using AppCore.Utilities;
using DG.Tweening;
using Domain.Entity;
using Domain.Enum;
using Presentation.Presenter;
using Presentation.Utilities;
using Presentation.Views.Extensions;
using Presentation.Views.Scene.Calendar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Popup
{
    public class ScheduleCreationPopup : PopupWindow
    {
        private const float HeightHeaderArea = 132f;
        private const float HeightInputFieldMargin = 13f;
        
        private enum Limit { Start, End }
        private enum Mode { New, Edit }

        [SerializeField] private RectTransform popupRectTransform;
        [SerializeField] private VerticalLayoutGroup scheduleContentGroup;
        [SerializeField] private ImageRx backgroundImage, hideBackgroundImage, hideButtonIcon, allDayIcon;
        [SerializeField] private ButtonRP hideButton;
        [SerializeField] private RectTransform scheduleTitleArea, scheduleDescriptionArea;
        [SerializeField] private TMP_InputField scheduleTitleField, scheduleDescriptionField;
        [SerializeField] private ButtonWithLabel startDateButton, startTimeButton, endDateButton, endTimeButton, repetitionButton;
        [SerializeField] private Toggle allDayToggle;

        private RectTransform _scheduleTitleRect, _scheduleDescriptionRect;

        private UnitSchedule _editSchedule;
        private Schedule _originSchedule;
        private Sequence _seq;

        private bool _isStateHide;
        private string _scheduleTitle, _scheduleDescription;
        private bool _titleUpdated, _descriptionUpdated;
        
        private Mode _mode;
        private CCDateOnly _startDate, _endDate;
        private CCTimeOnly _startTime, _endTime;
        private SchedulePeriodic _periodic;
        private bool _isAllDay;
        
        public void Init(CCDateOnly atDay, bool isAllDay)
        {
            _mode = Mode.New;
            
            var nowHour = CCTimeOnly.Now.Hour.Value;
            if (nowHour is 23)
            {
                _startTime = new CCTimeOnly();
                _endTime = new CCTimeOnly(1, 0, 0);
                _startDate = _endDate = atDay.AddDays(1);
            }
            else if (nowHour is 22)
            {
                _startTime = new CCTimeOnly(nowHour+1, 0, 0);
                _endTime = new CCTimeOnly();
                _startDate = atDay;
                _endDate = atDay.AddDays(1);
            }
            else
            {
                _startTime = new CCTimeOnly(nowHour + 1, 0, 0);
                _endTime = new CCTimeOnly(nowHour + 2, 0, 0);
                _startDate = _endDate = atDay;
            }
            
            _isAllDay = isAllDay;
            allDayToggle.isOn = isAllDay;
            
            ReloadAll();
        }

        public void Init(UnitSchedule schedule)
        {
            _editSchedule = schedule;
            _originSchedule = InAppContext.Context.GetService<ScheduleService>().FindSchedule(schedule.Id);
            _mode = Mode.Edit;
            
            _startDate = schedule.Duration.StartTime.ToDateOnly();
            _startTime = schedule.Duration.StartTime.ToTimeOnly();
            _endDate = schedule.Duration.EndTime.ToDateOnly();
            _endTime = schedule.Duration.EndTime.ToTimeOnly();
            
            _isAllDay = schedule.Duration.IsAllDay;
            allDayToggle.isOn = _isAllDay;

            _scheduleTitle = schedule.Title;
            _scheduleDescription = schedule.Description;
            
            _periodic = _originSchedule.Periodic;
            
            ReloadAll();
        }

        private void ReloadAll()
        {
            _scheduleTitleRect ??= scheduleTitleField.transform as RectTransform;
            _scheduleDescriptionRect ??= scheduleDescriptionField.transform as RectTransform;
            
            ToggleAllDay(_isAllDay);
            
            ReloadDateButtonLabel(Limit.Start);
            ReloadDateButtonLabel(Limit.End);
            ReloadTimeButtonLabel(Limit.Start);
            ReloadTimeButtonLabel(Limit.End);
            ReloadRepetitionLabel();

            scheduleTitleField.text = _scheduleTitle;
            scheduleDescriptionField.text = _scheduleDescription;

            hideButton.interactable = _mode is Mode.New;
        }

        private void Update()
        {
            const float marginHeightTitleArea = 40f;
            if (_titleUpdated)
            {
                var titleFieldHeight = scheduleTitleField.textComponent.GetRenderedValues(false).y;
                if (titleFieldHeight < 1f) titleFieldHeight = scheduleTitleField.textComponent.fontSize;
                titleFieldHeight += HeightInputFieldMargin;
                _scheduleTitleRect.sizeDelta = new Vector2(_scheduleTitleRect.sizeDelta.x, titleFieldHeight);
                scheduleTitleArea.sizeDelta = new Vector2(scheduleTitleArea.sizeDelta.x, titleFieldHeight + marginHeightTitleArea);

                LayoutRebuilder.ForceRebuildLayoutImmediate(scheduleContentGroup.transform as RectTransform);
                _titleUpdated = false;
            }

            if (_descriptionUpdated)
            {
                var titleFieldHeight = scheduleDescriptionField.textComponent.GetRenderedValues(false).y;
                if (titleFieldHeight < 1f) titleFieldHeight = scheduleDescriptionField.textComponent.fontSize;
                titleFieldHeight += HeightInputFieldMargin;
                _scheduleDescriptionRect.sizeDelta = new Vector2(_scheduleDescriptionRect.sizeDelta.x, titleFieldHeight);
                scheduleDescriptionArea.sizeDelta = new Vector2(scheduleDescriptionArea.sizeDelta.x, titleFieldHeight + marginHeightTitleArea);
        
                LayoutRebuilder.ForceRebuildLayoutImmediate(scheduleContentGroup.transform as RectTransform);
                _descriptionUpdated = false;
            }
        }

        public void OnPressHideButton()
        {
            const float timePopupHide = 0.25f;
            _seq?.Kill();
            _seq = DOTween.Sequence();
            
            _isStateHide = !_isStateHide;
            if (_isStateHide)
            {
                backgroundImage.enabled = false;
                hideBackgroundImage.enabled = true;
                hideButtonIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                var destinationHeight = safeRect.rect.height - (HeightHeaderArea + scheduleTitleArea.rect.height);
                _seq.Append(popupRectTransform.DOAnchorPosY(-destinationHeight, timePopupHide).SetEase(Ease.OutQuad));
            }
            else
            {
                hideButtonIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                _seq.Append(popupRectTransform.DOAnchorPosY(0, timePopupHide).SetEase(Ease.InQuad))
                    .OnComplete(() =>
                    {
                        backgroundImage.enabled = true;
                        hideBackgroundImage.enabled = false;
                    });
            }
        }

        public void SubmitScheduleWithClosing()
        {
            if (ReloadWarn(true)) return;

            var service = InAppContext.Context.GetService<ScheduleService>();
            if (_mode is Mode.New)
            {
                service.CreateSchedule(new Schedule(0, _scheduleTitle, _scheduleDescription, CreateDuration(), CreatePeriodic()));
                CloseWindow();
            }
            else
            {
            }
        }

        public void OnScheduleTitleChanged(string text)
        {
            _scheduleTitle = text;
            _titleUpdated = true;

            // LayoutRebuilder.ForceRebuildLayoutImmediate(scheduleTitleField.textComponent.transform as RectTransform);
            // var viewport = scheduleTitleField.textViewport;
            // for (var i = 0; i < viewport.childCount; i++)
            // {
            //     if (viewport.GetChild(i) is RectTransform rctf)
            //     {
            //         rctf.anchoredPosition = Vector2.zero;
            //     }
            // }
        }

        public void OnScheduleDescriptionChanged(string text)
        {
            _scheduleDescription = text;
            _descriptionUpdated = true;
            
            // var viewport = scheduleDescriptionField.textViewport;
            // for (var i = 0; i < viewport.childCount; i++)
            // {
            //     if (viewport.GetChild(i) is RectTransform rctf)
            //     {
            //         rctf.anchoredPosition = Vector2.zero;
            //     }
            // }
        }

        public void ToggleAllDay(bool isOn)
        {
            _isAllDay = isOn;

            if (isOn)
            {
                startTimeButton.Button.interactable = false;
                startTimeButton.Label.text = "";
                endTimeButton.Button.interactable = false;
                endTimeButton.Label.text = "";
                allDayIcon.colorType = ColorOf.TextDefault;
                ReloadWarn();
            }
            else
            {
                startTimeButton.Button.interactable = true;
                ReloadTimeButtonLabel(Limit.Start);
                endTimeButton.Button.interactable = true;
                ReloadTimeButtonLabel(Limit.End);
                allDayIcon.colorType = ColorOf.Background;
                ReloadWarn();
            }
        }

        [EnumAction(typeof(Limit))]
        public void OnPressDateButton(int intType)
        {
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<DateOnlyPopup>());
            if ((Limit)intType is Limit.Start)
            {
                window.Init(date =>
                {
                    _startDate = date;
                    ReloadDateButtonLabel(Limit.Start);
                    if (_startDate.CompareTo(_endDate) > 0)
                    {
                        _endDate = _startDate;
                        ReloadDateButtonLabel(Limit.End);
                    }
                    ReloadWarn();
                }, _startDate);
            }
            else
            {
                window.Init(date =>
                {
                    _endDate = date;
                    ReloadDateButtonLabel(Limit.End);
                    if (_endDate.CompareTo(_startDate) < 0)
                    {
                        _startDate = _endDate;
                        ReloadDateButtonLabel(Limit.Start);
                    }
                    ReloadWarn();
                }, _endDate);
            }
        }

        [EnumAction(typeof(Limit))]
        public void OnPressTimeButton(int intType)
        {
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<TimeOnlyPopup>());
            if ((Limit)intType is Limit.Start)
            {
                window.Init(time =>
                {
                    var diff = _endTime.WithDate(_endDate) - _startTime.WithDate(_startDate);
                    _startTime = time;
                    ReloadTimeButtonLabel(Limit.Start);
                    
                    var after = new CCDateTime(_startTime.WithDate(_startDate).AddSeconds(diff.TotalSeconds));
                    _endDate = after.ToDateOnly();
                    ReloadDateButtonLabel(Limit.End);
                    _endTime = after.ToTimeOnly();
                    ReloadTimeButtonLabel(Limit.End);
                    ReloadWarn();
                }, _startTime);
            }
            else
            {
                window.Init(time =>
                {
                    _endTime = time;
                    ReloadTimeButtonLabel(Limit.End);
                    ReloadWarn();
                }, _endTime);
            }
        }

        public void OnPressRepetitionButton()
        {
            var periodicDefault = _periodic ?? new SchedulePeriodic(SchedulePeriodicType.None, 1, _startDate);
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<PeriodicCreationPopup>());
            if (_originSchedule != null)
            {
                window.Init(periodic =>
                {
                    _periodic = periodic;
                    ReloadRepetitionLabel();
                }, periodicDefault);
            }
            else
            {
                window.Init(periodic =>
                {
                    _periodic = periodic;
                    ReloadRepetitionLabel();
                }, periodicDefault);
            }
        }

        private void ReloadDateButtonLabel(Limit type)
        {
            if (type is Limit.Start)
            {
                startDateButton.Label.text = _startDate.ToDateTime().ToString("yyyy年MM月dd日(ddd)", new CultureInfo("ja-JP"));
            }
            else
            {
                endDateButton.Label.text = _endDate.ToDateTime().ToString("yyyy年MM月dd日(ddd)", new CultureInfo("ja-JP"));
            }
        }

        private void ReloadTimeButtonLabel(Limit type)
        {
            if (type is Limit.Start)
            {
                startTimeButton.Label.text = _startTime.WithDate(CCDateOnly.Default).ToString("HH:mm");
            }
            else
            {
                endTimeButton.Label.text = _endTime.WithDate(CCDateOnly.Default).ToString("HH:mm");
            }
        }

        private void ReloadRepetitionLabel()
        {
            repetitionButton.Label.text = _periodic.ToExplainString();
        }
        
        private bool ReloadWarn(bool showWarnPopup = false)
        {
            var durationViolate = _isAllDay && _startDate.CompareTo(_endDate) > 0 
                                  || !_isAllDay && new CCDateTime(_startDate, _startTime) > new CCDateTime(_endDate, _endTime);

            if (durationViolate)
            {
                startDateButton.Button.imageRx.colorType = ColorOf.Danger;
                startTimeButton.Button.imageRx.colorType = ColorOf.Danger;
                endDateButton.Button.imageRx.colorType = ColorOf.Danger;
                endTimeButton.Button.imageRx.colorType = ColorOf.Danger;
            }
            else
            {
                startDateButton.Button.imageRx.colorType = ColorOf.Background;
                startTimeButton.Button.imageRx.colorType = ColorOf.Background;
                endDateButton.Button.imageRx.colorType = ColorOf.Background;
                endTimeButton.Button.imageRx.colorType = ColorOf.Background;
            }

            if (showWarnPopup)
            {
                var warnTexts = new List<string>();
                if (durationViolate)
                {
                    warnTexts.Add(_isAllDay ? "終了日時が開始日時より前になってはいけません。" : "終了時刻が開始時刻より前になってはいけません。");
                }

                if (warnTexts.Count > 0)
                {
                    PopupManager.Instance.ShowSinglePopup(string.Join('\n', warnTexts));
                }
            }
            
            return durationViolate;
        }
        
        private ScheduleDuration CreateDuration()
        {
            return _isAllDay
                ? new ScheduleDuration(_startDate, _endDate)
                : new ScheduleDuration(new CCDateTime(_startDate, _startTime), new CCDateTime(_endDate, _endTime));
        }

        private SchedulePeriodic CreatePeriodic()
        {
            return _periodic is null ? null : _periodic.PeriodicType is SchedulePeriodicType.None ? null : _periodic;
        }
    }
}