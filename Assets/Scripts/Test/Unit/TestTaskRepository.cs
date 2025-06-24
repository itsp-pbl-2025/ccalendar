using System.Linq;
using Infrastructure.Data.DAO;
using LiteDB;
using NUnit.Framework;
using Test.MockData;

namespace Test.Unit
{
    public class TestTaskRepository
    {
        [Test]
        public void TestInsert()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.TaskRepo;
            
            var index = 0;
            foreach (var dt in MockTask.GetMockTasksWithoutId())
            {
                var result = repo.Insert(dt.ToDomain());
                Assert.IsTrue(++index == result.Id);
            }

            Assert.Throws<LiteException>(() =>
            {
                repo.Insert(MockTask.GetMockTasks()[0].ToDomain());
            });
            
            ctx.Dispose();
        }

        [Test]
        public void TestUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.TaskRepo;
            
            Assert.IsFalse(repo.Update(MockTask.GetMockTasks()[0].ToDomain()));
            
            repo.Insert(MockTask.GetMockTasks()[0].ToDomain());
            var update = MockTask.GetMockTasks()[0];
            update.Title = "Updated Title";
            
            Assert.IsTrue(repo.Update(update.ToDomain()));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestInsertUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.TaskRepo;
            
            foreach (var dt in MockTask.GetMockTasks())
            {
                Assert.IsTrue(repo.InsertUpdate(dt.ToDomain()));
            }
            
            foreach (var dt in MockTask.GetMockTasks())
            {
                dt.Title += "(Updated)";
                Assert.IsFalse(repo.InsertUpdate(dt.ToDomain()));
            }
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestRemove()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.TaskRepo;
            
            Assert.IsFalse(repo.Remove(MockTask.GetMockTasks()[0].ToDomain()));
            
            foreach (var dt in MockTask.GetMockTasks())
            {
                repo.Insert(dt.ToDomain());
            }
            foreach (var dt in MockTask.GetMockTasks())
            {
                Assert.IsTrue(repo.Remove(dt.ToDomain()));
            }
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestGetAll()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.TaskRepo;
            
            foreach (var dt in MockTask.GetMockTasks())
            {
                repo.InsertUpdate(dt.ToDomain());
            }
            
            var tasks = repo.GetAll().ToDictionary(x => x.Id);
            foreach (var dt in MockTask.GetMockTasks())
            {
                Assert.IsTrue(tasks.TryGetValue(dt.Id, out var t));
                Assert.AreEqual(t, dt.ToDomain());
                Assert.IsTrue(tasks.Remove(dt.Id));
            }
            Assert.IsTrue(tasks.Count == 0);
            
            ctx.Dispose();
        }
    }
}