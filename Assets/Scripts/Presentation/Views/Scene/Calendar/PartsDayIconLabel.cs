﻿using System.Globalization;
using AppCore.UseCases;
using Domain.Entity;
using Presentation.Presenter;
using Presentation.Utilities;
using Presentation.Views.Extensions;
using UnityEngine;

namespace Presentation.Views.Scene.Calendar
{
    public class PartsDayIconLabel : MonoBehaviour
    {
        private enum ShowMode
        {
            Full,
            Mini
        }
        
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private LabelRx dateLabel;
        [SerializeField] private ImageRx bgImage;
        [SerializeField] private ShowMode mode;

        private HolidayService _holidayService;
        private CCDateOnly _date;

        public void Init(CCDateOnly date)
        {
            _holidayService ??= InAppContext.Context.GetService<HolidayService>();
            _date = date;

            dateLabel.colorType = date.ToDateTime().DayOfWeek.GetDayOfWeekColor();
            if (mode == ShowMode.Mini)
            {
                dateLabel.text = date.ToDateTime().ToString("MM/dd\nddd");
                if (_holidayService.IsHoliday(date))
                {
                    dateLabel.colorType = ColorOf.TextHoliday;
                }
            }
            else
            {
                if (_holidayService.TryGetHolidayName(date, out var holidayName))
                {
                    var dateString = date.ToDateTime().ToString("yyyy年 M月 d日 dddd", new CultureInfo("ja-JP"));
                    dateLabel.text = $"{holidayName}\n　{dateString}";
                    dateLabel.colorType = ColorOf.TextHoliday;
                }
                else
                {
                    dateLabel.text = date.ToDateTime().ToString("yyyy年 M月 d日 dddd", new CultureInfo("ja-JP"));
                }
            }
        }

        public CCDateOnly GetDate()
        {
            return _date;
        }

        public void SetAnchorX(float x)
        {
            rectTransform.anchorMin = new Vector2(x, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(x, rectTransform.anchorMax.y);
        }

        public float GetAnchorX()
        {
            return rectTransform.anchorMin.x;
        }
    }
}