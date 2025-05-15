using System.Linq;
using Infrastructure.Data.DAO;
using NUnit.Framework;
using Test.MockData;

namespace Test.Unit
{
    public class TestScheduleRepository
    {
        [Test]
        public void TestGetAll()
        {
            var ctx = InTestContext.Context;
            
            var repo = ctx.ScheduleRepo;
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                repo.InsertUpdate(ds.FromDomain());
            }
            
            var schedules = repo.GetAll().ToDictionary(x => x.Id);
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