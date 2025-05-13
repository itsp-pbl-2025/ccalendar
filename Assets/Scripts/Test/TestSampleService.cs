using System.Linq;
using AppCore.UseCases;
using Infrastructure.Data.DAO;
using NUnit.Framework;
using Test.MockData;

namespace Test
{
    public class TestSampleService
    {
        [Test]
        public void GetSchedules()
        {
            var ctx = InTestContext.Context;

            foreach (var d in MockSchedule.GetMockSchedules())
            {
                ctx.ScheduleRepo.InsertUpdate(ScheduleDao.FromDomain(d));
            }
            
            var service = ctx.GetService<SampleService>();
            var schedules = service.GetSchedules().ToDictionary(x => x.Id);
            foreach (var i in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(schedules.Remove(i.Id));
            }
            Assert.IsTrue(schedules.Count == 0);
            
            ctx.Dispose();
        }
    }
}