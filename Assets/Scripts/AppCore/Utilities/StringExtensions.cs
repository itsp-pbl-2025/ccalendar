using System;

namespace AppCore.Utilities
{
    public static class StringExtensions
    {
        public static string ToShortString(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "日",
                DayOfWeek.Monday => "月",
                DayOfWeek.Tuesday => "火",
                DayOfWeek.Wednesday => "水",
                DayOfWeek.Thursday => "木",
                DayOfWeek.Friday => "金",
                DayOfWeek.Saturday => "土",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }
        
        public static string ToLongString(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "日曜日",
                DayOfWeek.Monday => "月曜日",
                DayOfWeek.Tuesday => "火曜日",
                DayOfWeek.Wednesday => "水曜日",
                DayOfWeek.Thursday => "木曜日",
                DayOfWeek.Friday => "金曜日",
                DayOfWeek.Saturday => "土曜日",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }
    }
}