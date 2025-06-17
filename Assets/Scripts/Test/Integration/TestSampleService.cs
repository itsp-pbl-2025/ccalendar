using System.Linq;
using AppCore.UseCases;
using Infrastructure.Data.DAO;
using NUnit.Framework;
using Test.MockData;

namespace Test.Integration
{
    public class TestSampleService
    {
        [Test]
        public void GetSchedules()
        {
            var ctx = InTestContext.Context;

            foreach (var d in MockSchedule.GetMockSchedules())
            {
                ctx.ScheduleRepo.InsertUpdate(d.ToDomain());
            }
            
            var service = ctx.GetService<SampleService>();
            var schedules = service.GetSchedules().ToDictionary(x => x.Id);
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(schedules.TryGetValue(ds.Id, out var s));
                Assert.AreEqual(s, ds.ToDomain());
                Assert.IsTrue(schedules.Remove(ds.Id));
            }
            Assert.IsTrue(schedules.Count == 0);
            
            ctx.Dispose();
        }
    }
}