using System;

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
    }
}