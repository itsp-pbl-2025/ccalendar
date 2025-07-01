using AppCore.UseCases;
using Domain.Entity;
using Infrastructure.Data.DAO;
using NUnit.Framework;
using Test.MockData;
using ZLinq;

namespace Test.Integration
{
    public class TestTaskService
    {
        [Test]
        public void TestCreateTask()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();
            
            foreach (var dt in MockTask.GetMockTasksWithoutId())
            {
                service.CreateTask(dt.ToDomain());
            }
            
            var tasks = service.GetTask();
            Assert.AreEqual(MockTask.GetMockTasksWithoutId().Count, tasks.Count);
            
            ctx.Dispose();
        }

        [Test]
        public void TestUpdateTask()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();
            
            Assert.IsFalse(service.UpdateTask(MockTask.GetMockTasks().AsValueEnumerable().First().ToDomain()));
            foreach (var dt in MockTask.GetMockTasksWithoutId())
            {
                service.CreateTask(dt.ToDomain());
            }
            
            var createdTasks = service.GetTask();
            foreach (var task in createdTasks)
            {
                var updatedTask = new CCTask(task.Id, task.Title + "(Updated)", task.Description, task.Priority, task.Deadline);
                Assert.IsTrue(service.UpdateTask(updatedTask));
            }
            
            ctx.Dispose();
        }

        [Test]
        public void TestDeleteTask()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();
            
            Assert.IsFalse(service.DeleteTask(MockTask.GetMockTasks().AsValueEnumerable().First().ToDomain()));
            foreach (var dt in MockTask.GetMockTasksWithoutId())
            {
                service.CreateTask(dt.ToDomain());
            }
            
            var createdTasks = service.GetTask();
            foreach (var task in createdTasks)
            {
                Assert.IsTrue(service.DeleteTask(task));
            }
            Assert.IsTrue(service.GetTask().Count == 0);
            
            ctx.Dispose();
        }
        
        [Test]
        public void GetTask()
        {
            var ctx = InTestContext.Context;

            foreach (var d in MockTask.GetMockTasks())
            {
                ctx.TaskRepo.InsertUpdate(d.ToDomain());
            }
            
            var service = ctx.GetService<TaskService>();
            var tasks = service.GetTask().AsValueEnumerable().ToDictionary(x => x.Id);
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
