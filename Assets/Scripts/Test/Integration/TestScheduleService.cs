using System.Collections.Generic;
using AppCore.UseCases;
using Domain.Entity;
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
        public void TestFindSchedule()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();

            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                service.CreateSchedule(ds.ToDomain());
            }
            
            var schedules = MockSchedule.GetMockSchedules();
            var scheduleMax = schedules.Count;
            for (var i = 0; i < scheduleMax; i++)
            {
                var schedule = service.FindSchedule(i + 1);
                Assert.IsNotNull(schedule);
                Assert.AreEqual(schedule, schedules[i].ToDomain());
            }
            
            Assert.IsNull(service.FindSchedule(0));
            Assert.IsNull(service.FindSchedule(scheduleMax+1));
            
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
            var schedules = service.GetSchedulesInDuration(new ScheduleDuration(CCDateOnly.Today));
            foreach (var schedule in schedules)
            {
                Assert.IsTrue(okIds.Contains(schedule.Id));
                okIds.Remove(schedule.Id);
            }
            Assert.IsTrue(okIds.Count == 0);
            
            ctx.Dispose();
        }
    }
}