using System.Linq;
using AppCore.UseCases;
using Infrastructure.Data.DAO;
using LiteDB;
using NUnit.Framework;
using Test.MockData;

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
                service.CreateSchedule(ds.FromDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.Throws<LiteException>(() =>
                {
                    service.CreateSchedule(ds.FromDomain());
                });
            }
            
            ctx.Dispose();
        }

        [Test]
        public void TestUpdateSchedule()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();
            
            Assert.IsFalse(service.UpdateSchedule(MockSchedule.GetMockSchedules().First().FromDomain()));
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                service.CreateSchedule(ds.FromDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                ds.Title += "(Updated)";
                Assert.IsTrue(service.UpdateSchedule(ds.FromDomain()));
            }
            
            ctx.Dispose();
        }

        [Test]
        public void TestDeleteSchedule()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<ScheduleService>();
            
            Assert.IsFalse(service.DeleteSchedule(MockSchedule.GetMockSchedules().First().FromDomain()));
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                service.CreateSchedule(ds.FromDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(service.DeleteSchedule(ds.FromDomain()));
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
                ctx.ScheduleRepo.InsertUpdate(d.FromDomain());
            }
            
            var service = ctx.GetService<ScheduleService>();
            var schedules = service.GetSchedules().ToDictionary(x => x.Id);
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(schedules.TryGetValue(ds.Id, out var s));
                Assert.AreEqual(s, ds.FromDomain()); // interface同士の比較なのでoperator ==を使えない
                Assert.IsTrue(schedules.Remove(ds.Id));
            }
            Assert.IsTrue(schedules.Count == 0);
            
            ctx.Dispose();
        }
    }
}