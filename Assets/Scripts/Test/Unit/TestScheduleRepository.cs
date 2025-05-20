using System.Linq;
using Infrastructure.Data.DAO;
using LiteDB;
using NUnit.Framework;
using Test.MockData;

namespace Test.Unit
{
    public class TestScheduleRepository
    {
        [Test]
        public void TestInsert()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            var index = 0;
            foreach (var ds in MockSchedule.GetMockSchedulesWithoutId())
            {
                var result = repo.Insert(ds.FromDomain());
                Assert.IsTrue(++index == result.Id);
            }

            Assert.Throws<LiteException>(() =>
            {
                repo.Insert(MockSchedule.GetMockSchedules()[0].FromDomain());
            });
            
            ctx.Dispose();
        }

        [Test]
        public void TestUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            Assert.IsFalse(repo.Update(MockSchedule.GetMockSchedules()[0].FromDomain()));
            
            repo.Insert(MockSchedule.GetMockSchedules()[0].FromDomain());
            var update = MockSchedule.GetMockSchedules()[0];
            update.Title = "Updated Title";
            
            Assert.IsTrue(repo.Update(update.FromDomain()));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestInsertUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(repo.InsertUpdate(ds.FromDomain()));
            }
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                ds.Title += "(Updated)";
                Assert.IsFalse(repo.InsertUpdate(ds.FromDomain()));
            }
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestRemove()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            Assert.IsFalse(repo.Remove(MockSchedule.GetMockSchedules()[0].FromDomain()));
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                repo.Insert(ds.FromDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(repo.Remove(ds.FromDomain()));
            }
            
            ctx.Dispose();
        }
        
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