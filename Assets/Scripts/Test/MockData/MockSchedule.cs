using System;
using System.Collections.Generic;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.DAO;
using Infrastructure.Data.Schema;
using ZLinq;

namespace Test.MockData
{
    internal static class TestClock
    {
        /// <summary>擬似的な「今日」— テストを安定させるための固定日</summary>
        internal static readonly CCDateOnly Today = new(2025, 7, 15);

        /// <summary>擬似的な「今」— 0:00:00 で固定</summary>
        internal static CCDateTime Now =>
            new(Today.Year.Value, Today.Month.Value, Today.Day.Value, 0, 0, 0);
    }
    
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
                    new ScheduleDuration(TestClock.Now.AddHours(13), TestClock.Now.AddHours(16))),
                new(0,
                    "All Day Schedule",
                    "This is an all-day schedule. UwU",
                    new ScheduleDuration(TestClock.Today)),
                new(0,
                    "Every Week Schedule",
                    "Repeat it every week! UwU",
                    new ScheduleDuration(TestClock.Now.AddHours(17), TestClock.Now.AddHours(18)),
                    new SchedulePeriodic(
                        SchedulePeriodicType.EveryWeek,
                        Span: 1,
                        ExcludeIndices: new List<int> { 2 },   // ← 3回目だけ除外
                        StartDate: TestClock.Today,
                        EndDate: TestClock.Today.AddDays(30)
                    )),
                new(0, "", "", new ScheduleDuration(TestClock.Today.AddDays(10))),
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
                    new ScheduleDuration(TestClock.Now.AddHours(13), TestClock.Now.AddHours(16))),
                new(2,
                    "All Day Schedule",
                    "This is an all-day schedule. UwU",
                    new ScheduleDuration(TestClock.Today)),
                new(3,
                    "Every Week Schedule",
                    "Repeat it every week for a month! UwU",
                    new ScheduleDuration(TestClock.Now.AddHours(17), TestClock.Now.AddHours(18)),
                    new SchedulePeriodic(
                        SchedulePeriodicType.EveryWeek,
                        Span: 1,
                        ExcludeIndices: new List<int> { 2 },   // ← 3回目だけ除外
                        StartDate: TestClock.Today,
                        EndDate: TestClock.Today.AddDays(30)
                    )
                ),
                new(4, "", "", new ScheduleDuration(TestClock.Today.AddDays(10))),
            };

            return schedules.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
        }

        public static (List<DSchedule> ok, List<DSchedule> ng) GetMockSchedulesBoundary()
        {
            var ok = new List<Schedule>
            {
                new(0, "Normal Schedule", 
                    "1:00 ~ 2:00",
                    new ScheduleDuration(TestClock.Now.AddHours(1), TestClock.Now.AddHours(2))),
                new(0, "All Day Schedule",
                    "0:00:00 ~ 23:59:59",
                    new ScheduleDuration(TestClock.Today)),
                new(0, "Starting Boundary Schedule",
                    "-6:00 ~ 6:00",
                    new ScheduleDuration(TestClock.Now.AddHours(-6), TestClock.Now.AddHours(6))),
                new(0, "Ending Boundary Schedule",
                    "20:00 ~ 28:00",
                    new ScheduleDuration(TestClock.Now.AddHours(20), TestClock.Now.AddHours(28))),
                new(0, "Covering Schedule",
                    "-6:00 ~ 28:00",
                    new ScheduleDuration(TestClock.Now.AddHours(-6), TestClock.Now.AddHours(28))),
                new(0, "Repeating Schedule From 2 Days Ago",
                    "1:00 ~ 2:00",
                    new ScheduleDuration(TestClock.Now.AddDays(-2).AddHours(1), TestClock.Now.AddDays(-2).AddHours(2)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryDay, 1, TestClock.Now.ToDateOnly())),
                new(0, "Repeating Schedule By Every Weekday",
                    "1:00 ~ 2:00",
                    new ScheduleDuration(TestClock.Now.AddDays(-2).AddHours(1), TestClock.Now.AddDays(-2).AddHours(2)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryWeek, (1 << 7) - 1, TestClock.Now.ToDateOnly())),
            }.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
            var ng = new List<Schedule>
            {
                new(0, "Out Of Today Schedule",
                    "Today+1d",
                    new ScheduleDuration(TestClock.Today.AddDays(1))),
                new(0, "Out Of Month Schedule",
                    "Today+1M",
                    new ScheduleDuration(TestClock.Today.AddMonths(1))),
                new(0, "Straddle Schedule",
                    "1:00 ~ 2:00",
                    new ScheduleDuration(TestClock.Now.AddDays(-1).AddHours(1), TestClock.Now.AddDays(-1).AddHours(2)),
                    new SchedulePeriodic(SchedulePeriodicType.EveryDay, 2, TestClock.Now.ToDateOnly())),
            }.AsValueEnumerable().Select(e => e.FromDomain()).ToList();
            
            return (ok, ng);
        }
    }
}