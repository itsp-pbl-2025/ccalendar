using System;
using System.Collections.Generic;
using Domain.Enum;
using Infrastructure.Data.Schema;

namespace Test.MockData
{
    public static class MockSchedule
    {
        public static List<DSchedule> GetMockSchedulesWithoutId()
        {
            return new List<DSchedule>
            {
                new DSchedule
                {
                    Title = "Duration Schedule",
                    Description = "Hello! Here's description. OwO",
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(13),
                        EndTime = DateTime.Today.AddHours(16),
                        IsAllDay = false,
                    },
                },
                new DSchedule
                {
                    Title = "All Day Schedule",
                    Description = "This is an all-day schedule. UwU",
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(9),
                        EndTime = DateTime.Today.AddHours(11),
                        IsAllDay = true,
                    },
                },
                new DSchedule
                {
                    Title = "Every Week Schedule",
                    Description = "Repeat it every week! UwU",
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(17),
                        EndTime = DateTime.Today.AddHours(18),
                        IsAllDay = false,
                    },
                    Periodic = new DSchedulePeriodic()
                    {
                        PeriodicType = SchedulePeriodicType.EveryWeek,
                        Span = 1,
                    }
                },
                new DSchedule
                { // only duration
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(2),
                        EndTime = DateTime.Today.AddHours(4),
                    },
                },
            };
        }
        
        public static List<DSchedule> GetMockSchedules()
        {
            return new List<DSchedule>
            {
                new DSchedule
                {
                    Id = 1,
                    Title = "Duration Schedule",
                    Description = "Hello! Here's description. OwO",
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(13),
                        EndTime = DateTime.Today.AddHours(16),
                        IsAllDay = false,
                    },
                },
                new DSchedule
                {
                    Id = 2,
                    Title = "All Day Schedule",
                    Description = "This is an all-day schedule. UwU",
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(9),
                        EndTime = DateTime.Today.AddHours(11),
                        IsAllDay = true,
                    },
                },
                new DSchedule
                {
                    Id = 3,
                    Title = "Every Week Schedule",
                    Description = "Repeat it every week! UwU",
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(17),
                        EndTime = DateTime.Today.AddHours(18),
                        IsAllDay = false,
                    },
                    Periodic = new DSchedulePeriodic()
                    {
                        PeriodicType = SchedulePeriodicType.EveryWeek,
                        Span = 1,
                    }
                },
                new DSchedule
                { // only duration
                    Id = 4,
                    Duration = new DScheduleDuration()
                    {
                        StartTime = DateTime.Today.AddHours(2),
                        EndTime = DateTime.Today.AddHours(4),
                    },
                },
            };
        }
    }
}