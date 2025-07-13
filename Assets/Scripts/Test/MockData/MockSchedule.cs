using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.DAO;
using Infrastructure.Data.Schema;
using ZLinq;

namespace Test.MockData
{
    public static class MockSchedule
    {
        public static List<DSchedule> GetMockSchedulesWithoutId()
        {
            // ID自動割り当て
            List<Schedule> schedules = new List<Schedule>
            {
                new(0,
                    "Duration Schedule",
                    "Hello! Here's description. OwO",
                    new ScheduleDuration(CCDateTime.Today.AddHours(13), CCDateTime.Today.AddHours(16))),
                new(0,
                    "All Day Schedule",
                    "This is an all-day schedule. UwU",
                    new ScheduleDuration(CCDateOnly.Today)),
                new(0,
                    "Every Week Schedule",
                    "Repeat it every week! UwU",
                    new ScheduleDuration(CCDateTime.Today.AddHours(17), CCDateTime.Today.AddHours(18)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeek, 1)),
                new(0, "", "", new ScheduleDuration(CCDateOnly.Today.AddDays(10))),
            };

            return schedules.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
        }

        public static List<DSchedule> GetMockSchedules()
        {
            // ID手動割り当て
            List<Schedule> schedules = new List<Schedule>
            {
                new(1,
                    "Duration Schedule",
                    "Hello! Here's description. OwO",
                    new ScheduleDuration(CCDateTime.Today.AddHours(13), CCDateTime.Today.AddHours(16))),
                new(2,
                    "All Day Schedule",
                    "This is an all-day schedule. UwU",
                    new ScheduleDuration(CCDateOnly.Today)),
                new(3,
                    "Every Week Schedule",
                    "Repeat it every week! UwU",
                    new ScheduleDuration(CCDateTime.Today.AddHours(17), CCDateTime.Today.AddHours(18)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeek, 1)),
                new(4, "", "", new ScheduleDuration(CCDateOnly.Today.AddDays(10))),
            };

            return schedules.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
        }

        public static (List<DSchedule> ok, List<DSchedule> ng) GetMockSchedulesBoundary()
        {
            var ok = new List<Schedule>
            {
                new(0, "Normal Schedule", 
                    "1:00 ~ 2:00",
                    new ScheduleDuration(CCDateTime.Today.AddHours(1), CCDateTime.Today.AddHours(2))),
                new(0, "All Day Schedule",
                    "0:00:00 ~ 23:59:59",
                    new ScheduleDuration(CCDateOnly.Today)),
                new(0, "Starting Boundary Schedule",
                    "-6:00 ~ 6:00",
                    new ScheduleDuration(CCDateTime.Today.AddHours(-6), CCDateTime.Today.AddHours(6))),
                new(0, "Ending Boundary Schedule",
                    "20:00 ~ 28:00",
                    new ScheduleDuration(CCDateTime.Today.AddHours(20), CCDateTime.Today.AddHours(28))),
                new(0, "Covering Schedule",
                    "-6:00 ~ 28:00",
                    new ScheduleDuration(CCDateTime.Today.AddHours(-6), CCDateTime.Today.AddHours(28))),
                new(0, "Repeating Schedule From 2 Days Ago",
                    "1:00 ~ 2:00",
                    new ScheduleDuration(CCDateTime.Today.AddDays(-2).AddHours(1), CCDateTime.Today.AddDays(-2).AddHours(2)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryDay, 1)),
                new(0, "Repeating Schedule By Every Weekday",
                    "1:00 ~ 2:00",
                    new ScheduleDuration(CCDateTime.Today.AddDays(-2).AddHours(1), CCDateTime.Today.AddDays(-2).AddHours(2)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeekday, (1 << 7) - 1)),
            }.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
            var ng = new List<Schedule>
            {
                new(0, "Out Of Today Schedule",
                    "Today+1d",
                    new ScheduleDuration(CCDateOnly.Today.AddDays(1))),
                new(0, "Out Of Month Schedule",
                    "Today+1M",
                    new ScheduleDuration(CCDateOnly.Today.AddMonths(1))),
                new(0, "Straddle Schedule",
                    "1:00 ~ 2:00",
                    new ScheduleDuration(CCDateTime.Today.AddDays(-1).AddHours(1), CCDateTime.Today.AddDays(-1).AddHours(2)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryDay, 2)),
            }.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
            
            return (ok, ng);
        }
    }
}