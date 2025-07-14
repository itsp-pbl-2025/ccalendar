using Infrastructure.Data.DAO;
using LiteDB;
using NUnit.Framework;
using Test.MockData;
using ZLinq;

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
                var result = repo.Insert(ds.ToDomain());
                Assert.IsTrue(++index == result.Id);
            }

            Assert.Throws<LiteException>(() =>
            {
                repo.Insert(MockSchedule.GetMockSchedules()[0].ToDomain());
            });
            
            ctx.Dispose();
        }

        [Test]
        public void TestUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            Assert.IsFalse(repo.Update(MockSchedule.GetMockSchedules()[0].ToDomain()));
            
            repo.Insert(MockSchedule.GetMockSchedules()[0].ToDomain());
            var update = MockSchedule.GetMockSchedules()[0];
            update.Title = "Updated Title";
            
            Assert.IsTrue(repo.Update(update.ToDomain()));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestInsertUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(repo.InsertUpdate(ds.ToDomain()));
            }
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                ds.Title += "(Updated)";
                Assert.IsFalse(repo.InsertUpdate(ds.ToDomain()));
            }
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestRemove()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            Assert.IsFalse(repo.Remove(MockSchedule.GetMockSchedules()[0].ToDomain()));
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                repo.Insert(ds.ToDomain());
            }
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                Assert.IsTrue(repo.Remove(ds.ToDomain()));
            }
            
            ctx.Dispose();
        }

        [Test]
        public void TestGet()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;

            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                repo.InsertUpdate(ds.ToDomain());
            }

            var schedules = MockSchedule.GetMockSchedules();
            var scheduleMax = schedules.Count;
            for (var i = 0; i < scheduleMax; i++)
            {
                var schedule = repo.Get(i + 1);
                Assert.IsNotNull(schedule);
                Assert.AreEqual(schedule, schedules[i].ToDomain());
            }
            
            Assert.IsNull(repo.Get(0));
            Assert.IsNull(repo.Get(scheduleMax+1));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestGetAll()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.ScheduleRepo;
            
            foreach (var ds in MockSchedule.GetMockSchedules())
            {
                repo.InsertUpdate(ds.ToDomain());
            }
            
            var schedules = repo.GetAll().AsValueEnumerable().ToDictionary(x => x.Id);
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