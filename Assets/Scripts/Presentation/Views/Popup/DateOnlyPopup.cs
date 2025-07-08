using System;
using System.Collections.Generic;
using Domain.Entity;
using Presentation.Utilities;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Popup
{
    public class DateOnlyPopup : PopupWindow
    {
        [SerializeField] private LabelRx dateText, monthText;
        [SerializeField, Tooltip("7x6")] private List<ButtonWithLabel> dateButtons;
        [SerializeField] private ButtonRP monthLeft, monthRight, monthDefault;
        
        private Action<CCDateOnly> _onDateDefined;

        private readonly Dictionary<int, CCDateOnly> _targetDateDictionary = new();
        
        private CCDateOnly _selectedDateOnly = CCDateOnly.Default;
        private CCDateOnly _sinceDateOnly = CCDateOnly.Default, _untilDateOnly = CCDateOnly.MaxValue;
        private int _targetYear, _targetMonth;

        public void Init(Action<CCDateOnly> onDateOnlyDefined, CCDateOnly? target = null, CCDateOnly? selected = null)
        {
            _onDateDefined = onDateOnlyDefined;
            _selectedDateOnly = selected ?? CCDateOnly.Default;

            var focusMonth = target ?? CCDateOnly.Today;
            
            SetMonth(focusMonth.Year.Value, focusMonth.Month.Value);

            for (var i = 0; i < dateButtons.Count; i++)
            {
                var index = i;
                dateButtons[i].Button.onClick.AddListener(() =>
                {
                    if (_targetDateDictionary.TryGetValue(index, out var day))
                    {
                        OnPressDateButton(day);
                    }
                });
            }
        }

        private void Start()
        {
            Reload();
        }

        public void SetLimitationSince(CCDateOnly since)
        {
            _sinceDateOnly = since;
            Reload();
        }
        
        public void SetLimitationUntil(CCDateOnly until)
        {
            _untilDateOnly = until;
            Reload();
        }

        private void OnPressDateButton(CCDateOnly selectedDate)
        {
            _selectedDateOnly = selectedDate;
            if (selectedDate.Year.Value != _targetYear || selectedDate.Month.Value != _targetMonth)
            {
                SetMonth(selectedDate.Year.Value, selectedDate.Month.Value);
            }
            else
            {
                Reload();
            }
        }
        
        public void OnPressMoveMonthButton(bool isLeft)
        {
            if (isLeft)
            {
                if (_targetMonth == 1)
                {
                    SetMonth(_targetYear - 1, 12);
                }
                else
                {
                    SetMonth(_targetYear, _targetMonth - 1);
                }
            }
            else
            {
                if (_targetMonth == 12)
                {
                    SetMonth(_targetYear + 1, 1);
                }
                else
                {
                    SetMonth(_targetYear, _targetMonth + 1);
                }
            }
        }

        public void OnPressDefaultButton()
        {
            var now = CCDateOnly.Today;
            SetMonth(now.Year.Value, now.Month.Value);
        }

        public void OnPressDefineButton()
        {
            _onDateDefined?.Invoke(_selectedDateOnly);
            CloseWindow();
        }

        private void SetMonth(int year, int month)
        {
            _targetYear = year;
            _targetMonth = month;
            
            Reload();
        }

        private void Reload()
        {
            dateText.text = _selectedDateOnly.IsDefault() ? "未選択" : _selectedDateOnly.ToDateTime().ToString("yyyy年 MM月 dd日");
            monthText.text = $"{_targetYear}年{_targetMonth}月";
            
            var month1 = new CCDateOnly(_targetYear, _targetMonth, 1);
            var week = month1.ToDateTime().DayOfWeek;
            var indexDate = month1.AddDays(-(int)week);
            
            _targetDateDictionary.Clear();
            for (var i = 0; i < dateButtons.Count; i++)
            {
                var buttonDate = indexDate.AddDays(i);
                _targetDateDictionary.Add(i, buttonDate);
                var button = dateButtons[i];
                var outOfMonth = buttonDate.Month.Value != _targetMonth || buttonDate.Year.Value != _targetYear;
                var outOfRange = buttonDate.CompareTo(_sinceDateOnly) < 0 || _untilDateOnly.CompareTo(buttonDate) < 0;
                button.Label.text = buttonDate.Day.Value.ToString();
                button.Button.interactable = !outOfRange;
                button.Button.imageRx.colorType = ColorOf.Surface;
                if (_selectedDateOnly.CompareTo(buttonDate) == 0)
                {
                    button.Button.Select();
                    button.Button.imageRx.colorType = ColorOf.Secondary;
                }
                var buttonDayOfWeek = buttonDate.ToDateTime().DayOfWeek;
                button.Label.colorType = 
                    outOfRange ? ColorOf.TextDisabled :
                    outOfMonth ? ColorOf.TextTertiary : 
                    buttonDayOfWeek is DayOfWeek.Saturday ? ColorOf.TextSaturday :
                    buttonDayOfWeek is DayOfWeek.Sunday ? ColorOf.TextHoliday : ColorOf.TextDefault;
            }
            
            monthLeft.interactable = month1.AddDays(-1).CompareTo(_sinceDateOnly) > 0;
            monthRight.interactable = month1.AddMonths(1).CompareTo(_untilDateOnly) < 0;
            
            var now = CCDateOnly.Today;
            monthDefault.interactable = _targetYear != now.Year.Value || _targetMonth != now.Month.Value;
        }
    }
}