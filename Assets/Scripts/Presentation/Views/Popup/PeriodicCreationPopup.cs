using System;
using System.Collections.Generic;
using AppCore.Utilities;
using Domain.Entity;
using Domain.Enum;
using Presentation.Presenter;
using Presentation.Views.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Views.Popup
{
    public class PeriodicCreationPopup : PopupWindow
    {
        private enum ValuePeriodicType
        {
            None,
            EveryDay,
            EveryWeek,
            EveryMonth,
            EveryYear,
        }

        private enum ValueMonthdayType
        {
            SpecifiedDay,
            WeekIndex,
            FinalWeek,
        }

        private enum ValueEndDateType
        {
            Endless,
            DateLimited,
            IndexLimited,
        }
        
        [SerializeField] private RectTransform windowBaseRect;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private List<ToggleWithLabel> periodicTypeToggles, weekdayToggles, monthToggles, endDayToggles;
        [SerializeField] private GameObject intervalSettings, weekdaySettings, monthSettings;
        [SerializeField] private LabelRx intervalText;
        [SerializeField] private CanvasGroup endDateRayGroup, endIndexRayGroup;
        [SerializeField] private ButtonWithLabel endDatePopupButton;
        [SerializeField] private TMP_InputField intervalInputField, endIndexInputField;
        
        private RectTransform _verticalLayoutRect;
        private readonly Dictionary<ValuePeriodicType, ToggleWithLabel> _periodicTypeToggles = new();
        private readonly Dictionary<DayOfWeek, ToggleWithLabel> _weekdayToggles = new();
        private readonly Dictionary<ValueMonthdayType, ToggleWithLabel> _monthToggles = new();
        private readonly Dictionary<ValueEndDateType, ToggleWithLabel> _endDayToggles = new();
        
        private Action<SchedulePeriodic> _callback;
        
        private ValuePeriodicType _periodicType;
        private int _span;
        private readonly Dictionary<DayOfWeek, bool> _weekdaySet = new();
        private ValueMonthdayType _monthType;
        private int _dayOfMonthSpecified;
        private (int index, DayOfWeek dayOfWeek, bool isFinalWeek) _weekIndexSpecified;
        private ValueEndDateType _endDayType;
        private CCDateOnly _startDate, _endDate = CCDateOnly.MaxValue;
        private int _endIndex;
        private bool _initialized, _needReload;

        public void Init(Action<SchedulePeriodic> onDefineCallback, Schedule originSchedule = null)
        {
            _callback = onDefineCallback;

            var periodic = originSchedule?.Periodic;
            _periodicType = periodic is not null ? periodic.PeriodicType switch
                {
                    SchedulePeriodicType.EveryDay => ValuePeriodicType.EveryDay,
                    SchedulePeriodicType.EveryWeek => ValuePeriodicType.EveryWeek,
                    SchedulePeriodicType.EveryMonth => ValuePeriodicType.EveryMonth,
                    SchedulePeriodicType.EveryYear => ValuePeriodicType.EveryYear,
                    _ => ValuePeriodicType.None
                } : ValuePeriodicType.None;
            
            // 毎週と毎月はSpanを特殊に使うので1に設定
            _span = _periodicType is ValuePeriodicType.EveryWeek or ValuePeriodicType.EveryMonth ? 1 : periodic?.Span ?? 1;

            // 毎週設定を作成
            var startDate = originSchedule is null ? DateTime.Today : originSchedule.Duration.StartTime.ToDateTime();
            _startDate = new CCDateTime(startDate).ToDateOnly();
            var weekdayFlag = _periodicType is ValuePeriodicType.EveryWeek ? periodic?.Span ?? 0 : 0;
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                _weekdaySet.Add(dayOfWeek, (weekdayFlag & (1 << (int)dayOfWeek)) != 0);
            }

            if (_periodicType is not ValuePeriodicType.EveryWeek)
            {
                _weekdaySet[startDate.DayOfWeek] = true;
            }
            
            // 毎月設定を作成
            var monthlyFlag = _periodicType is ValuePeriodicType.EveryMonth ? periodic?.Span ?? 0 : 0;
            _monthType = monthlyFlag >= 500 ? ValueMonthdayType.FinalWeek : 
                monthlyFlag >= 100 ? ValueMonthdayType.WeekIndex : ValueMonthdayType.SpecifiedDay;

            _dayOfMonthSpecified = _monthType is ValueMonthdayType.SpecifiedDay && monthlyFlag > 0 ? monthlyFlag : startDate.Day;
            _weekIndexSpecified = _monthType is not ValueMonthdayType.SpecifiedDay && monthlyFlag > 0
                ? (monthlyFlag % 100, (DayOfWeek)(monthlyFlag / 100), monthlyFlag % 100 > 4)
                : new CCDateOnly(startDate).GetDayOfWeekWithIndex();
            
            // TODO: 終了日設定を作成
            _endDayType = ValueEndDateType.Endless;
            _endDate = new CCDateTime(startDate).AddYears(1).ToDateOnly();
            _endIndex = 1;
            
            ReloadInit();
        }

        private void ReloadInit()
        {
            _verticalLayoutRect = windowBaseRect.transform as RectTransform;
            
            endDateRayGroup.blocksRaycasts = _endDayType is ValueEndDateType.DateLimited;
            endDatePopupButton.Button.interactable = _endDayType is ValueEndDateType.DateLimited;
            endIndexRayGroup.blocksRaycasts = _endDayType is ValueEndDateType.IndexLimited;
            endIndexInputField.interactable = _endDayType is ValueEndDateType.IndexLimited;

            foreach (ValuePeriodicType periodicType in Enum.GetValues(typeof(ValuePeriodicType)))
            {
                _periodicTypeToggles.Add(periodicType, periodicTypeToggles[(int)periodicType]);
            }

            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                _weekdayToggles.Add(dayOfWeek, weekdayToggles[(int)dayOfWeek]);
            }

            foreach (ValueMonthdayType monthdayType in Enum.GetValues(typeof(ValueMonthdayType)))
            {
                _monthToggles.Add(monthdayType, monthToggles[(int)monthdayType]);
            }

            foreach (ValueEndDateType endDateType in Enum.GetValues(typeof(ValueEndDateType)))
            {
                _endDayToggles.Add(endDateType, endDayToggles[(int)endDateType]);
            }
            
            intervalInputField.text = _span.ToString();
            endIndexInputField.text = _endIndex.ToString();
            
            endDatePopupButton.Label.text = _endDate.ToDateTime().ToString("yyyy年MM月dd日");
            
            _monthToggles[ValueMonthdayType.SpecifiedDay].Label.text = $"毎月{_dayOfMonthSpecified}日";
            _monthToggles[ValueMonthdayType.WeekIndex].Label.text = $"第{_weekIndexSpecified.index}{_weekIndexSpecified.dayOfWeek.ToLongString()}";
            _monthToggles[ValueMonthdayType.FinalWeek].Label.text = $"最終{_weekIndexSpecified.dayOfWeek.ToLongString()}";
            
            _monthToggles[ValueMonthdayType.WeekIndex].gameObject.SetActive(_weekIndexSpecified.index <= 4);
            _monthToggles[ValueMonthdayType.FinalWeek].gameObject.SetActive(_weekIndexSpecified.isFinalWeek);

            ReloadAll();
            
            // トグルボタンの初期化
            foreach (ValuePeriodicType periodic in Enum.GetValues(typeof(ValuePeriodicType)))
            {
                _periodicTypeToggles[periodic].Toggle.isOn = periodic == _periodicType;
            }
            foreach (var (dayOfWeek, on) in _weekdaySet)
            {
                _weekdayToggles[dayOfWeek].Toggle.isOn = on;
            }
            _monthToggles[_monthType].Toggle.isOn = true;
            _endDayToggles[_endDayType].Toggle.isOn = true;
            _initialized = true;
        }

        private void ReloadAll()
        {
            switch (_periodicType)
            {
                case ValuePeriodicType.EveryDay:
                case ValuePeriodicType.EveryYear:
                    intervalSettings.SetActive(true);
                    weekdaySettings.SetActive(false);
                    monthSettings.SetActive(false);
                    break;
                case ValuePeriodicType.EveryWeek:
                    intervalSettings.SetActive(false);
                    weekdaySettings.SetActive(true);
                    monthSettings.SetActive(false);
                    break;
                case ValuePeriodicType.EveryMonth:
                    intervalSettings.SetActive(false);
                    weekdaySettings.SetActive(false);
                    monthSettings.SetActive(true);
                    break;
                case ValuePeriodicType.None:
                default:
                    intervalSettings.SetActive(false);
                    weekdaySettings.SetActive(false);
                    monthSettings.SetActive(false);
                    break;
            }

            if (_verticalLayoutRect)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutRect);
                Canvas.ForceUpdateCanvases();
            }
            windowBaseRect.sizeDelta = new Vector2(windowBaseRect.sizeDelta.x, verticalLayoutGroup.preferredHeight);
        }

        private void LateUpdate()
        {
            if (_needReload)
            {
                _needReload = false;
            }
        }

        #region OnPressPeriodicTypeToggles
        
        public void OnPressPeriodicTypeNone(bool toggle) => ToggleScheduleType(ValuePeriodicType.None, toggle);
        public void OnPressPeriodicTypeEveryDay(bool toggle) => ToggleScheduleType(ValuePeriodicType.EveryDay, toggle);
        public void OnPressPeriodicTypeEveryWeek(bool toggle) => ToggleScheduleType(ValuePeriodicType.EveryWeek, toggle);
        public void OnPressPeriodicTypeEveryMonth(bool toggle) => ToggleScheduleType(ValuePeriodicType.EveryMonth, toggle);
        public void OnPressPeriodicTypeEveryYear(bool toggle) => ToggleScheduleType(ValuePeriodicType.EveryYear, toggle);
        
        private void ToggleScheduleType(ValuePeriodicType type, bool toggle)
        {
            if (toggle) _periodicType = type;
            _periodicTypeToggles[type].Toggle.interactable = !toggle;

            intervalText.text = _periodicType is ValuePeriodicType.EveryYear ? "年に1回" : "日に1回";
            
            if (_initialized) ReloadAll();
        }

        #endregion
        
        #region OnPressDayOfWeekToggles

        public void OnPressDayOfWeekSunday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Sunday, toggle);
        public void OnPressDayOfWeekMonday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Monday, toggle);
        public void OnPressDayOfWeekTuesday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Tuesday, toggle);
        public void OnPressDayOfWeekWednesday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Wednesday, toggle);
        public void OnPressDayOfWeekThursday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Thursday, toggle);
        public void OnPressDayOfWeekFriday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Friday, toggle);
        public void OnPressDayOfWeekSaturday(bool toggle) => ToggleDayOfWeek(DayOfWeek.Saturday, toggle);
        
        private void ToggleDayOfWeek(DayOfWeek dayOfWeek, bool toggle)
        {
            if (_initialized) _weekdaySet[dayOfWeek] = toggle;
        }

        #endregion

        #region OnPressMonthdayTypeToggles

        public void OnPressMonthdayTypeSpecifiedDay(bool toggle) => ToggleMonthdayType(ValueMonthdayType.SpecifiedDay, toggle);
        public void OnPressMonthdayTypeWeekIndex(bool toggle) => ToggleMonthdayType(ValueMonthdayType.WeekIndex, toggle);
        public void OnPressMonthdayTypeFinalWeek(bool toggle) => ToggleMonthdayType(ValueMonthdayType.FinalWeek, toggle);
        
        private void ToggleMonthdayType(ValueMonthdayType type, bool toggle)
        {
            if (toggle) _monthType = type;
            _monthToggles[type].Toggle.interactable = !toggle;
        }

        #endregion

        #region OnPressEndDateTypeToggles

        public void OnPressEndDateTypeEndless(bool toggle) => ToggleEndDateType(ValueEndDateType.Endless, toggle);
        public void OnPressEndDateTypeDateLimited(bool toggle) => ToggleEndDateType(ValueEndDateType.DateLimited, toggle);
        public void OnPressEndDateTypeIndexLimited(bool toggle) => ToggleEndDateType(ValueEndDateType.IndexLimited, toggle);
        
        private void ToggleEndDateType(ValueEndDateType type, bool toggle)
        {
            if (toggle) _endDayType = type;

            switch (type)
            {
                case ValueEndDateType.Endless:
                    break;
                case ValueEndDateType.DateLimited:
                    endDateRayGroup.blocksRaycasts = toggle;
                    endDatePopupButton.Button.interactable = toggle;
                    break;
                case ValueEndDateType.IndexLimited:
                    endIndexRayGroup.blocksRaycasts = toggle;
                    endIndexInputField.interactable = toggle;
                    break;
            }
        }

        public void EditEndDateLimited()
        {
            var window = PopupManager.Instance.ShowPopup(InAppContext.Prefabs.GetPopup<DateOnlyPopup>());
            window.Init(date =>
            {
                _endDate = date;
                endDatePopupButton.Label.text = date.ToDateTime().ToString("yyyy年MM月dd日");
            }, _endDate);
            window.SetLimitationSince(_startDate);
        }

        #endregion

        public void SubmitPeriodicWithClosing()
        {
            if (_callback is not null)
            {
                SchedulePeriodic periodic = null;
                switch (_periodicType)
                {
                    case ValuePeriodicType.None:
                        // null
                        break;
                    case ValuePeriodicType.EveryDay:
                        if (int.TryParse(intervalInputField.text, out _span) && _span > 0)
                        {
                            periodic = new SchedulePeriodic(SchedulePeriodicType.EveryDay, _span);
                        }
                        else
                        {
                            periodic = new SchedulePeriodic(SchedulePeriodicType.EveryDay, 1);
                        }
                        break;
                    case ValuePeriodicType.EveryWeek:
                        var weekSpan = 0;
                        foreach (var weekDay in _weekdaySet)
                        {
                            weekSpan += 1 << (int)weekDay.Key;
                        }
                        periodic = new SchedulePeriodic(SchedulePeriodicType.EveryWeek, weekSpan);
                        break;
                    case ValuePeriodicType.EveryMonth:
                        var monthSpan = _monthType switch
                        {
                            ValueMonthdayType.SpecifiedDay => _dayOfMonthSpecified,
                            ValueMonthdayType.FinalWeek => 500 + (int)_weekIndexSpecified.dayOfWeek,
                            _ => _weekIndexSpecified.index * 100 + (int)_weekIndexSpecified.dayOfWeek
                        };
                        periodic = new SchedulePeriodic(SchedulePeriodicType.EveryMonth, monthSpan);
                        break;
                    case ValuePeriodicType.EveryYear:
                        if (int.TryParse(intervalInputField.text, out _span) && _span > 0)
                        {
                            periodic = new SchedulePeriodic(SchedulePeriodicType.EveryYear, _span);
                        }
                        else
                        {
                            periodic = new SchedulePeriodic(SchedulePeriodicType.EveryYear, 1);
                        }
                        break;
                }
                _callback.Invoke(periodic);
            }
            
            CloseWindow();
        }
    }
}