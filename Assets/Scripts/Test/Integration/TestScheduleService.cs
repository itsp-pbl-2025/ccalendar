using AppCore.UseCases;
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
    }
}