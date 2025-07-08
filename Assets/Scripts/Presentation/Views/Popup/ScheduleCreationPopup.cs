using System;
using System.Collections.Generic;
using System.Globalization;
using AppCore.UseCases;
using DG.Tweening;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Utilities;
using Presentation.Views.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Popup
{
    public class ScheduleCreationPopup : PopupWindow
    {
        private const float HeightHeaderArea = 120f;
        private const float HeightInputFieldMargin = 13f;
        
        private enum Limit { Start, End }

        [SerializeField] private RectTransform popupRectTransform;
        [SerializeField] private VerticalLayoutGroup scheduleContentGroup;
        [SerializeField] private ImageRx backgroundImage, hideBackgroundImage, hideButtonIcon, allDayIcon;
        [SerializeField] private RectTransform scheduleTitleArea, scheduleDescriptionArea;
        [SerializeField] private TMP_InputField scheduleTitleField, scheduleDescriptionField;
        [SerializeField] private ButtonWithLabel startDateButton, startTimeButton, endDateButton, endTimeButton, repetitionButton;
        [SerializeField] private Toggle allDayToggle;

        private RectTransform _scheduleTitleRect, _scheduleDescriptionRect;

        private Sequence _seq;

        private bool _isStateHide;
        private string _scheduleTitle, _scheduleDescription;
        private bool _titleUpdated, _descriptionUpdated;
        
        private CCDateOnly _startDate, _endDate;
        private CCTimeOnly _startTime, _endTime;
        private bool _isAllDay;
        
        public void Init(CCDateOnly atDay, bool isAllDay)
        {
            _startDate = _endDate = atDay;
            _startTime = new CCTimeOnly(CCTimeOnly.Now.Hour.Value+1, 0, 0);
            _endTime = new CCTimeOnly(CCTimeOnly.Now.Hour.Value+2, 0, 0);
            
            ReloadDateButtonLabel(Limit.Start);
            ReloadDateButtonLabel(Limit.End);
            ReloadTimeButtonLabel(Limit.Start);
            ReloadTimeButtonLabel(Limit.End);
            
            _isAllDay = isAllDay;
            
            allDayToggle.isOn = isAllDay;
            ToggleAllDay(isAllDay);
            
            _scheduleTitleRect = scheduleTitleField.transform as RectTransform;
            _scheduleDescriptionRect = scheduleDescriptionField.transform as RectTransform;
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
            var duration = _isAllDay
                ? new ScheduleDuration(_startDate, _endDate)
                : new ScheduleDuration(new CCDateTime(_startDate, _startTime), new CCDateTime(_endDate, _endTime));
            
            service.CreateSchedule(new Schedule(0, _scheduleTitle, _scheduleDescription, duration));
            
            CloseWindow();
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
            throw new NotImplementedException();
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
    }
}