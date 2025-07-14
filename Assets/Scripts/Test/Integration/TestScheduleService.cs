using System.Collections.Generic;
using AppCore.UseCases;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.DAO;
using LiteDB;
using NUnit.Framework;
using Test.MockData;
using ZLinq;
using System.Reflection;

namespace Test.Integration
{
    public class TestScheduleService
    {
        [Test]
        public void TestCreateSchedule()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                service.CreateSchedule(ds.ToDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.Throws<LiteException>(() =>
                {
                    service.CreateSchedule(ds.ToDomain());
                });
            }
            
            ctx.Dispose();
        }

        [Test]
        public void TestUpdateSchedule()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();
            
            Assert.IsFalse(service.UpdateSchedule(MockSchedule.GetMockSchedules().AsValueEnumerable().First().ToDomain()));
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                service.CreateSchedule(ds.ToDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                ds.Title += "(Updated)";
                Assert.IsTrue(service.UpdateSchedule(ds.ToDomain()));
            }
            
            ctx.Dispose();
        }

        [Test]
        public void TestDeleteSchedule()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();
            
            Assert.IsFalse(service.DeleteSchedule(MockSchedule.GetMockSchedules().AsValueEnumerable().First().ToDomain()));
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                service.CreateSchedule(ds.ToDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(service.DeleteSchedule(ds.ToDomain()));
            }
            Assert.IsTrue(service.GetSchedules().Count == 0);
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestGetSchedules()
        {
            var ctx = InTestContext.Context;

            foreach (var d in MockSchedule.GetMockSchedules())
            {
                ctx.ScheduleRepo.InsertUpdate(d.ToDomain());
            }
            
            var service = ctx.GetService<ScheduleService>();
            var schedules = service.GetSchedules().AsValueEnumerable().ToDictionary(x => x.Id);
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(schedules.TryGetValue(ds.Id, out var s));
                Assert.AreEqual(s, ds.ToDomain()); // interface同士の比較なのでoperator ==を使えない
                Assert.IsTrue(schedules.Remove(ds.Id));
            }
            Assert.IsTrue(schedules.Count == 0);
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestGetSchedulesInDuration()
        {
            var ctx = InTestContext.Context;

            var (ok, ng) = MockSchedule.GetMockSchedulesBoundary();
            var okIds = new HashSet<int>();

            foreach (var dok in ok)
            {
                var result = ctx.ScheduleRepo.Insert(dok.ToDomain());
                okIds.Add(result.Id);
            }
            foreach (var dng in ng)
            {
                ctx.ScheduleRepo.InsertUpdate(dng.ToDomain());
            }
            
            var service = ctx.GetService<ScheduleService>();
            var schedules = service.GetSchedulesInDuration(new ScheduleDuration(TestClock.Today));
            foreach (var schedule in schedules)
            {
                Assert.IsTrue(okIds.Contains(schedule.Id));
                okIds.Remove(schedule.Id);
            }
            Assert.IsTrue(okIds.Count == 0);
            
            ctx.Dispose();
        }
        
        [Test]
        public void GetSchedulesInDuration_ShouldSkip_ExcludedIndices()
        {
            var ctx     = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();

            // 2025/07/15 〜 2025/07/18 の 4 回発生する Daily
            var startDate = new CCDateOnly(2025, 7, 15);

            var periodic = new SchedulePeriodic(
                SchedulePeriodicType.EveryDay,
                Span: 1,
                ExcludeIndices: new List<int> { 1, 3 },   // ← 2 回目と 4 回目を除外
                StartDate: startDate,
                EndDate:   startDate.AddDays(3));         // index = 0-3

            var duration = new ScheduleDuration(
                new CCDateTime(2025, 7, 15,  9, 0, 0),
                new CCDateTime(2025, 7, 15, 10, 0, 0));

            service.CreateSchedule(new Schedule(
                0, "DailySkip", "", duration, periodic));

            // 問合せ窓は 4 日間
            var window = new ScheduleDuration(
                new CCDateTime(2025, 7, 15,  0, 0, 0),
                new CCDateTime(2025, 7, 18, 23, 59, 59));

            var result = service.GetSchedulesInDuration(window);

            // index 0 と 2 の 2 件だけ返る
            Assert.AreEqual(2, result.Count);

            ctx.Dispose();
        }
        
        [Test]
        public void GetSchedulesInDuration_ShouldStop_AtPeriodicEnd()
        {
            var ctx     = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();

            var startDate = new CCDateOnly(2025, 7, 15);

            var periodic = new SchedulePeriodic(
                SchedulePeriodicType.EveryDay,
                Span: 1,
                ExcludeIndices: new List<int>(),          // 除外なし
                StartDate: startDate,
                EndDate:   startDate.AddDays(1));         // 2 日間だけ

            var duration = new ScheduleDuration(
                new CCDateTime(2025, 7, 15, 8, 0, 0),
                new CCDateTime(2025, 7, 15, 9, 0, 0));

            service.CreateSchedule(new Schedule(
                0, "ShortDaily", "", duration, periodic));

            // 窓は 2 週間と広いが
            var window = new ScheduleDuration(
                new CCDateTime(2025, 7, 15, 0, 0, 0),
                new CCDateTime(2025, 7, 31, 0, 0, 0));

            var result = service.GetSchedulesInDuration(window);

            // EndDate=+1d までの 2 件しか返ってこない
            Assert.AreEqual(2, result.Count);

            ctx.Dispose();
        }
        
        [Test]
        public void GetSchedulesInDuration_ShouldReuse_DurationCache()
        {
            var ctx     = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();

            var periodic = new SchedulePeriodic(
                SchedulePeriodicType.EveryWeek,
                Span: 1,
                ExcludeIndices: new List<int>(),
                StartDate: TestClock.Today,
                EndDate:   TestClock.Today.AddDays(90));

            var duration = new ScheduleDuration(
                new CCDateTime(TestClock.Today.Year.Value,
                    TestClock.Today.Month.Value,
                    TestClock.Today.Day.Value,
                    12, 0, 0),
                new CCDateTime(TestClock.Today.Year.Value,
                    TestClock.Today.Month.Value,
                    TestClock.Today.Day.Value,
                    13, 0, 0));

            service.CreateSchedule(new Schedule(0, "Weekly", "", duration, periodic));

            var window = new ScheduleDuration(TestClock.Today);

            // 1 回目でキャッシュ生成
            service.GetSchedulesInDuration(window);

            // リフレクションで _durationCache を取得
            var cacheField = typeof(ScheduleService)
                .GetField("_durationCache", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var cache = (Dictionary<int, SortedDictionary<int, ScheduleDuration>>)cacheField.GetValue(service)!;

            Assert.AreEqual(1, cache.Count);                 // スケジュールは 1 つ
            var inner = cache.Values.AsValueEnumerable().First();
            var firstCount = inner.Count;                    // 生成された Duration の数

            // 2 回目：同じ窓で取得しても増えない
            service.GetSchedulesInDuration(window);
            Assert.AreEqual(firstCount, inner.Count);

            ctx.Dispose();
        }
        
        [Test]
        public void IsInRange_WithEndDate_WorksAsExpected()
        {
            var start = new CCDateOnly(2025, 7, 10);
            var end = new CCDateOnly(2025, 7, 20);
            var periodic = new SchedulePeriodic(SchedulePeriodicType.EveryDay, 1, start, end);

            Assert.IsFalse(periodic.IsInRange(new CCDateOnly(2025, 7, 9)));  // before start
            Assert.IsTrue (periodic.IsInRange(new CCDateOnly(2025, 7, 10))); // exact start
            Assert.IsTrue (periodic.IsInRange(new CCDateOnly(2025, 7, 15))); // between
            Assert.IsTrue (periodic.IsInRange(new CCDateOnly(2025, 7, 20))); // exact end
            Assert.IsFalse(periodic.IsInRange(new CCDateOnly(2025, 7, 21))); // after end
        }
        [Test]
        public void IsInRange_WithoutEndDate_WorksAsExpected()
        {
            var start = new CCDateOnly(2025, 7, 10);
            var periodic = new SchedulePeriodic(SchedulePeriodicType.EveryDay, 1, start, null);

            Assert.IsFalse(periodic.IsInRange(new CCDateOnly(2025, 7, 9)));  // before start
            Assert.IsTrue (periodic.IsInRange(new CCDateOnly(2025, 7, 10))); // exact start
            Assert.IsTrue (periodic.IsInRange(new CCDateOnly(2100, 1, 1)));  // far future
        }
    }
}