using System;
using Domain.Entity;

namespace AppCore.Utilities
{
    public static class DateTimeExtensions
    {
        public static (int index, DayOfWeek dayOfWeek, bool isFinalWeek) GetDayOfWeekWithIndex(this CCDateOnly dateOnly)
        {
            var index = dateOnly.Day.Value / 7 + 1;
            var dayOfWeek = dateOnly.ToDateTime().DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(dateOnly.Year.Value, dateOnly.Month.Value);
            var isFinalWeek = daysInMonth - dateOnly.Day.Value < 7;
            return (index, dayOfWeek, isFinalWeek);
        }

        public static CCDateOnly GetIndexedWeekDay(int year, int month, int index, DayOfWeek dayOfWeek)
        {
            var monthFirst = new DateTime(year, month, 1);
            if ((int)monthFirst.DayOfWeek <= (int)dayOfWeek) index -= 1;
            var indexFirst = monthFirst.AddDays(-(int)monthFirst.DayOfWeek);
            return new CCDateOnly(indexFirst.AddDays(index * 7 + (int)dayOfWeek));
        }

        public static CCDateOnly GetFinalWeekDay(int year, int month, DayOfWeek dayOfWeek)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var monthLastWeekFrom = new DateTime(year, month, daysInMonth - 6);
            var stepIndex = -(int)monthLastWeekFrom.DayOfWeek;
            if ((int)monthLastWeekFrom.DayOfWeek > (int)dayOfWeek) stepIndex += 7;
            return new CCDateOnly(monthLastWeekFrom.AddDays(stepIndex + (int)dayOfWeek));
        }

        public static CCDateOnly GetDayFromIndices(int year, int month, int index, DayOfWeek dayOfWeek)
        {
            return index > 4 ? GetFinalWeekDay(year, month, dayOfWeek) : GetIndexedWeekDay(year, month, index, dayOfWeek);
        }
    }
}