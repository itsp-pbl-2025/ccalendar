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
                    new ScheduleDuration(DateTime.Today.AddHours(13), DateTime.Today.AddHours(16))),
                new(0,
                    "All Day Schedule",
                    "This is an all-day schedule. UwU",
                    new ScheduleDuration()),
                new(0,
                    "Every Week Schedule",
                    "Repeat it every week! UwU",
                    new ScheduleDuration(DateTime.Today.AddHours(17), DateTime.Today.AddHours(18)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeek, 1)),
                new(0, "", "", new ScheduleDuration()),
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
                    new ScheduleDuration(DateTime.Today.AddHours(13), DateTime.Today.AddHours(16))),
                new(2,
                    "All Day Schedule",
                    "This is an all-day schedule. UwU",
                    new ScheduleDuration()),
                new(3,
                    "Every Week Schedule",
                    "Repeat it every week! UwU",
                    new ScheduleDuration(DateTime.Today.AddHours(17), DateTime.Today.AddHours(18)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeek, 1)),
                new(4, "", "", new ScheduleDuration()),
            };

            return schedules.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
        }
    }
}