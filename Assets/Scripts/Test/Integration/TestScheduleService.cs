using AppCore.UseCases;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.DAO;
using LiteDB;
using NUnit.Framework;
using Test.MockData;
using ZLinq;

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
        public void GetSchedules()
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