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
        
        private Action<CCDateTime> _onDateDefined;

        private readonly Dictionary<int, CCDateTime> _targetDateDictionary = new();
        
        private CCDateTime _selectedDateTime;
        private CCDateTime _sinceDateTime = CCDateTime.MinValue, _untilDateTime = CCDateTime.MaxValue;
        private int _targetYear, _targetMonth;
        
        public void Init(Action<CCDateTime> onDateTimeDefined, CCDateTime target = default)
        {
            _onDateDefined = onDateTimeDefined;
            if (target == default) target = CCDateTime.Now;
            
            SetMonth(target.Year.Value, target.Month.Value);

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

        public void SetLimitationSince(CCDateTime since)
        {
            _sinceDateTime = since;
            Reload();
        }
        
        public void SetLimitationUntil(CCDateTime until)
        {
            _untilDateTime = until;
            Reload();
        }

        private void OnPressDateButton(CCDateTime selectedDate)
        {
            _selectedDateTime = selectedDate;
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
            var now = CCDateTime.Now;
            SetMonth(now.Year.Value, now.Month.Value);
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
            
            var month1 = new CCDateTime(_targetYear, _targetMonth, 1);
            var week = month1.DayOfWeek;
            var indexDate = month1.AddDays(-(int)week);
            
            _targetDateDictionary.Clear();
            for (var i = 0; i < dateButtons.Count; i++)
            {
                var buttonDate = indexDate.AddDays(i);
                _targetDateDictionary.Add(i, buttonDate);
                var button = dateButtons[i];
                var outOfMonth = buttonDate.Month.Value != _targetMonth || buttonDate.Year.Value != _targetYear;
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
                    outOfRange ? ColorOf.TextDisabled :
                    outOfMonth ? ColorOf.TextTertiary : 
                    buttonDate.DayOfWeek is DayOfWeek.Saturday ? ColorOf.TextSaturday :
                    buttonDate.DayOfWeek is DayOfWeek.Sunday ? ColorOf.TextHoliday : ColorOf.TextDefault;
            }
            
            monthLeft.interactable = month1.AddDays(-1) > _sinceDateTime;
            monthRight.interactable = month1.AddMonths(1) < _untilDateTime;
            
            var now = CCDateTime.Now;
            monthDefault.interactable = _targetYear != now.Year.Value || _targetMonth != now.Month.Value;
        }
    }
}