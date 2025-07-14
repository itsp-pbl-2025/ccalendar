using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Enum;
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
        [SerializeField] private List<GameObject> underEndDateObjects;
        [SerializeField] private ToggleWithLabel toggleEndDateLimited, toggleEndIndexLimited;
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
        private Dictionary<DayOfWeek, bool> _weekdaySet;
        private ValueMonthdayType _monthType;
        private ValueEndDateType _endDayType;

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
            var weekdayFlag = _periodicType is ValuePeriodicType.EveryWeek ? periodic?.Span ?? 0 : 0;
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                _weekdaySet.Add(dayOfWeek, (weekdayFlag & (1 << (int)dayOfWeek)) != 0);
            }

            if (_periodicType is not ValuePeriodicType.EveryWeek)
            {
                var startDate = originSchedule is null ? DateTime.Today : originSchedule.Duration.StartTime.ToDateTime();
                _weekdaySet[startDate.DayOfWeek] = true;
            }
            
            // 毎月設定を作成
            
            
            ReloadInit();
        }

        private void ReloadInit()
        {
            _verticalLayoutRect = windowBaseRect.transform as RectTransform;

            foreach (ValuePeriodicType periodicType in Enum.GetValues(typeof(ValuePeriodicType)))
            {
                _periodicTypeToggles.Add(periodicType, periodicTypeToggles[(int)periodicType]);
            }

            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                _weekdayToggles.Add(dayOfWeek, periodicTypeToggles[(int)dayOfWeek]);
            }

            foreach (ValueMonthdayType monthdayType in Enum.GetValues(typeof(ValueMonthdayType)))
            {
                _monthToggles.Add(monthdayType, monthToggles[(int)monthdayType]);
            }

            foreach (ValueEndDateType endDateType in Enum.GetValues(typeof(ValueEndDateType)))
            {
                _endDayToggles.Add(endDateType, endDayToggles[(int)endDateType]);
            }

            ReloadAll();
        }

        private void ReloadAll()
        {
            if (_verticalLayoutRect)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutRect);

                if (_verticalLayoutRect.childCount == 0)
                {
                    Debug.LogWarning("AnyPeriodicPopup doesn't have any content.");
                    return;
                }
            
                var lastChildRect = _verticalLayoutRect.GetChild(_verticalLayoutRect.childCount-1) as RectTransform;

                if (lastChildRect == null)
                {
                    Debug.LogWarning("AnyPeriodicPopup's Last Child is not RectTransform-ed-Object.");
                    return;
                }
            
                windowBaseRect.sizeDelta = new Vector2(windowBaseRect.sizeDelta.x,
                    Mathf.Abs(lastChildRect.anchoredPosition.y) + lastChildRect.sizeDelta.y);
            }
        }

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
                        periodic = new SchedulePeriodic(SchedulePeriodicType.EveryDay, _span);
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
                        var monthSpan = 0;
                        // TODO: Fill HERE
                        periodic = new SchedulePeriodic(SchedulePeriodicType.EveryMonth, monthSpan);
                        break;
                    case ValuePeriodicType.EveryYear:
                        periodic = new SchedulePeriodic(SchedulePeriodicType.EveryYear, _span);
                        break;
                }
                _callback.Invoke(periodic);
            }
            
            CloseWindow();
        }
    }
}