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
            
            var text = "";
            if (periodic.EndDate is not null)
            {
                text += periodic.EndDate.Value.ToDateTime().ToString("yyyy年MM月dd日") + "まで";
            }

            var span = periodic.Span;
            switch (periodic.PeriodicType)
            {
                case SchedulePeriodicType.EveryDay:
                    text += span is 1 ? "毎日" : $"{span}日に1回";
                    break;
                case SchedulePeriodicType.EveryWeek:
                    var weekdays = new List<string>();
                    foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        if ((span & (1 << (int)day)) != 0)
                        {
                            weekdays.Add(day.ToShortString());
                        }
                    }
                    if (weekdays.Count is 0) return noRepeat;
                    text += weekdays.Count is 7 ? "毎日" : $"毎週{string.Join(',', weekdays)}曜日";
                    break;
                case SchedulePeriodicType.EveryMonth:
                    if (span >= 100)
                    {
                        var index = span / 100;
                        var day = (DayOfWeek)(span % 100);
                        text += $"{(index > 4 ? "毎月最終" : $"毎月第{index}")}{day.ToLongString()}";
                    }
                    else
                    {
                        text += $"毎月{span}日";
                    }
                    break;
                case SchedulePeriodicType.EveryYear:
                    text += $"{span}年に1回";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return text;
        }
    }
}