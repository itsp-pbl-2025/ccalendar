using System;
using System.Collections.Generic;
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
        
        private Action<DateTime> _onDateDefined;

        private readonly Dictionary<int, DateTime> _targetDateDictionary = new();
        
        private DateTime _selectedDateTime;
        private DateTime _sinceDateTime = DateTime.MinValue, _untilDateTime = DateTime.MaxValue;
        private int _targetYear, _targetMonth;
        
        public void Init(Action<DateTime> onDateTimeDefined, DateTime target = default)
        {
            _onDateDefined = onDateTimeDefined;
            if (target == default) target = DateTime.Now;
            
            SetMonth(target.Year, target.Month);

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

        public void SetLimitationSince(DateTime since)
        {
            _sinceDateTime = since;
            Reload();
        }
        
        public void SetLimitationUntil(DateTime until)
        {
            _untilDateTime = until;
            Reload();
        }

        private void OnPressDateButton(DateTime selectedDate)
        {
            _selectedDateTime = selectedDate;
            if (selectedDate.Year != _targetYear || selectedDate.Month != _targetMonth)
            {
                SetMonth(selectedDate.Year, selectedDate.Month);
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
            var now = DateTime.Now;
            SetMonth(now.Year, now.Month);
        }

        public void OnPressDefineButton()
        {
            _onDateDefined?.Invoke(_selectedDateTime);
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
            dateText.text = _selectedDateTime == default ? "未選択" : _selectedDateTime.ToString("yyyy年 MM月 dd日");
            monthText.text = $"{_targetYear}年{_targetMonth}月";
            
            var month1 = new DateTime(_targetYear, _targetMonth, 1);
            var week = month1.DayOfWeek;
            var indexDate = month1.AddDays(-(int)week);
            
            _targetDateDictionary.Clear();
            for (var i = 0; i < dateButtons.Count; i++)
            {
                var buttonDate = indexDate.AddDays(i);
                _targetDateDictionary.Add(i, buttonDate);
                var button = dateButtons[i];
                var outOfMonth = buttonDate.Month != _targetMonth || buttonDate.Year != _targetYear;
                var outOfRange = buttonDate < _sinceDateTime || buttonDate > _untilDateTime;
                button.Label.text = buttonDate.Day.ToString();
                button.Button.interactable = !outOfRange;
                button.Button.imageRx.colorType = ColorOf.Surface;
                if (buttonDate == _selectedDateTime)
                {
                    button.Button.Select();
                    button.Button.imageRx.colorType = ColorOf.Secondary;
                }
                button.Label.colorType = 
                    outOfRange ? ColorOf.TextDisabled : outOfMonth ? ColorOf.TextTertiary : 
                    buttonDate.DayOfWeek is DayOfWeek.Saturday ? ColorOf.TextSaturday :
                    buttonDate.DayOfWeek is DayOfWeek.Sunday ? ColorOf.TextHoliday : ColorOf.TextDefault;
            }
            
            monthLeft.interactable = month1.AddDays(-1) > _sinceDateTime;
            monthRight.interactable = month1.AddMonths(1) < _untilDateTime;
            
            var now = DateTime.Now;
            monthDefault.interactable = _targetYear != now.Year || _targetMonth != now.Month;
        }
    }
}