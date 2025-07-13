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
        private const float HeightScheduleInWeek = 40f;
        
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private ImageRx todayPointer;
        [SerializeField] private List<LabelRx> dateLabels;
        [SerializeField] private List<RectTransform> scheduleParents;

        private HolidayService _holidayService;

        public void Init(CCDateOnly date, bool isFirstRow = false)
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

                var showMonth = (isFirstRow && dayOfWeek is DayOfWeek.Sunday) || day.Day.Value == 1;
                label.text = showMonth ? day.ToDateTime().ToString("M/d") : day.Day.Value.ToString();

                if (today.Equals(day))
                {
                    todayPointer.rectTransform.anchorMin =
                        new Vector2(label.rectTransform.anchorMin.x, todayPointer.rectTransform.anchorMin.y);
                    todayPointer.rectTransform.anchorMax =
                        new Vector2(label.rectTransform.anchorMax.x, todayPointer.rectTransform.anchorMax.y);
                    todayPointer.rectTransform.anchoredPosition =
                        new Vector2(0f, todayPointer.rectTransform.anchoredPosition.y);
                    todayPointer.enabled = true;
                }
            }
        }

        private void SetAnchorY(float min, float max)
        {
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, max);
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, min);
        }

        public void SetOffset(int offset)
        {
            SetAnchorY((5 - offset) / 6f, (6 - offset) / 6f);
        }
        
        public void PlaceSchedules(int column, List<PartsScheduleInDay> schedules, bool instant)
        {
            var parent = scheduleParents[column];

            var offset = 0f;
            foreach (var schedule in schedules)
            {
                schedule.SetParent(parent);
                schedule.TransformInWeek(offset, HeightScheduleInWeek, instant);
                offset += HeightScheduleInWeek;
            }
        }
    }
}