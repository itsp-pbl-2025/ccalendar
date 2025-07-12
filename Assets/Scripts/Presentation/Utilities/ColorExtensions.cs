using System;
using UnityEngine;

namespace Presentation.Utilities
{
    public static class ColorExtensions
    {
        public static ColorOf GetDayOfWeekColor(this DayOfWeek dayOfWeek, ColorOf defaultColor = ColorOf.TextDefault)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday => ColorOf.TextSaturday,
                DayOfWeek.Sunday => ColorOf.TextHoliday,
                _ => defaultColor
            };
        }

        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}