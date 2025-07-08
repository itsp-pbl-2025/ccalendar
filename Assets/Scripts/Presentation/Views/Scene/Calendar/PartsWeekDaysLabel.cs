using System;
using System.Collections.Generic;
using AppCore.UseCases;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Utilities;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class PartsWeekDaysLabel : MonoBehaviour
    {
        [SerializeField] public RectTransform rectTransform;
        [SerializeField] public ImageRx todayPointer;
        [SerializeField] public List<LabelRx> dateLabels;

        private HolidayService _holidayService;

        private void Init(CCDateOnly date, bool isFirstRow = false)
        {
            _holidayService ??= InAppContext.Context.GetService<HolidayService>();

            todayPointer.enabled = false;
            var today = CCDateOnly.Today;
            var sunDate = date.AddDays(-(int)date.ToDateTime().DayOfWeek);
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var label = dateLabels[(int)dayOfWeek];
                var day = sunDate.AddDays((int)dayOfWeek);

                var labelColor = dayOfWeek.GetDayOfWeekColor();
                if (_holidayService.IsHoliday(day)) labelColor = ColorOf.TextHoliday;
                label.colorType = labelColor;

                var showMonth = isFirstRow || day.Day.Value == 1;
                label.text = showMonth ? day.ToDateTime().ToString("M/d") : day.Day.Value.ToString();

                if (today.Equals(day))
                {
                    todayPointer.rectTransform.anchoredPosition = label.rectTransform.anchoredPosition;
                    todayPointer.enabled = true;
                }
            }
        }

        public void SetAnchorY(float y)
        {
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, y);
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, y);
        }
    }
}