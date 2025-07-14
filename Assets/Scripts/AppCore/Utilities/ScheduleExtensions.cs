using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Enum;
using ZLinq;

namespace AppCore.Utilities
{
    public static class ScheduleExtensions
    {
        public static string ToExplainString(this SchedulePeriodic periodic)
        {
            const string noRepeat = "繰り返しなし";
            if (periodic is null) return noRepeat;

            var span = periodic.Span;
            switch (periodic.PeriodicType)
            {
                case SchedulePeriodicType.EveryDay:
                    if (span is 1)
                    {
                        return "毎日";
                    }
                    return $"{span}日に1回";
                case SchedulePeriodicType.EveryWeekday:
                    var weekdays = new List<DayOfWeek>();
                    foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        if ((span & (1 << (int)day)) != 0)
                        {
                            weekdays.Add(day);
                        }
                    }
                    if (weekdays.Count is 0) return noRepeat;
                    if (weekdays.Count is 7) return "毎日";
                    var joinedDay = string.Join(',', weekdays.AsValueEnumerable().Select(d => d.ToShortString()));
                    return $"毎週{joinedDay}曜日";
                case SchedulePeriodicType.EveryWeek:
                    return "毎週";
                case SchedulePeriodicType.EveryMonth:
                    if (span >= 100)
                    {
                        var index = span / 100;
                        var day = (DayOfWeek)(span % 100);
                        return $"{(index > 4 ? "最終" : $"第{index}")}{day.ToLongString()}";
                    }
                    return $"毎月{span}日";
                case SchedulePeriodicType.EveryYear:
                    return $"{span}年に1回";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}