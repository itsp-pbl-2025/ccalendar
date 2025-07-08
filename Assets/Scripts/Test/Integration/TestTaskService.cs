using System;
using System.Linq;
using AppCore.UseCases;
using Domain.Entity;
using Domain.Enum;
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
        
        [Test]
        public void TestCompleteTask_NonPeriodic()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();

            // 登録前は存在しないので false
            var np = new CCTask(
                Id: 1,
                Title: "NonPeriodic",
                Description: "Desc",
                Priority: 1,
                Deadline: CCDateTime.Today.AddDays(1)
            );
            Assert.IsFalse(service.CompleteTask(np));

            // タスク登録 → 完了
            service.CreateTask(np);
            var saved = service.GetTask().First(t => t.Id == 1);
            Assert.IsTrue(service.CompleteTask(saved));

            var completed = service.GetTask().First(t => t.Id == 1);
            Assert.IsTrue(completed.IsCompleted);

            ctx.Dispose();
        }

        [Test]
        public void TestCompleteTask_EveryWeek()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();

            var start = CCDateTime.Today;
            var weekly = new CCTask(
                Id: 1,
                Title: "WeeklyTask",
                Description: "週次タスク",
                Priority: 2,
                Deadline: start,
                Duration: CCTimeSpan.FromHours(1),
                Periodic: new TaskPeriodic(TaskPeriodicType.EveryWeek, 1)
            );
            service.CreateTask(weekly);

            var saved = service.GetTask().First(t => t.Id == 1);
            Assert.IsTrue(service.CompleteTask(saved));

            var all = service.GetTask();
            // 元タスクが完了フラグ付き
            var orig = all.First(t => t.Id == 1);
            Assert.IsTrue(orig.IsCompleted);

            // 新タスクが生成され、完了フラグなし＆7日後の期限になってる
            var nxt = all.First(t => t.Id != 1);
            Assert.IsFalse(nxt.IsCompleted);
            Assert.AreEqual(start.AddDays(7), nxt.Deadline);

            ctx.Dispose();
        }

        [Test]
        public void TestCompleteTask_EveryWeekday()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();

            // 今日から次の金曜日を取得
            var today = CCDateTime.Today;
            var friday = today;
            while (friday.DayOfWeek != DayOfWeek.Friday)
                friday = friday.AddDays(1);

            var wd = new CCTask(
                Id: 1,
                Title: "WeekdayTask",
                Description: "平日タスク",
                Priority: 3,
                Deadline: friday,
                Duration: CCTimeSpan.FromHours(2),
                Periodic: new TaskPeriodic(TaskPeriodicType.EveryWeekday, 1)
            );
            service.CreateTask(wd);

            var saved = service.GetTask().First(t => t.Id == 1);
            Assert.IsTrue(service.CompleteTask(saved));

            var all = service.GetTask();
            var newTask = all.First(t => t.Id != 1);
            // 金曜→月曜(3日後) になってるはず
            Assert.AreEqual(friday.AddDays(3), newTask.Deadline);

            ctx.Dispose();
        }

        [Test]
        public void TestCompleteTask_EveryMonth()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();

            var start = CCDateTime.Today;
            var monthly = new CCTask(
                Id: 1,
                Title: "MonthlyTask",
                Description: "月次タスク",
                Priority: 1,
                Deadline: start,
                Duration: CCTimeSpan.FromHours(1),
                Periodic: new TaskPeriodic(TaskPeriodicType.EveryMonth, 1)
            );
            service.CreateTask(monthly);

            var saved = service.GetTask().First(t => t.Id == 1);
            Assert.IsTrue(service.CompleteTask(saved));

            var newTask = service.GetTask().First(t => t.Id != 1);
            Assert.AreEqual(start.AddMonths(1), newTask.Deadline);

            ctx.Dispose();
        }

        [Test]
        public void TestCompleteTask_EveryYear()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<TaskService>();

            var start = CCDateTime.Today;
            var yearly = new CCTask(
                Id: 1,
                Title: "YearlyTask",
                Description: "年次タスク",
                Priority: 1,
                Deadline: start,
                Duration: CCTimeSpan.FromHours(1),
                Periodic: new TaskPeriodic(TaskPeriodicType.EveryYear, 1)
            );
            service.CreateTask(yearly);

            var saved = service.GetTask().First(t => t.Id == 1);
            Assert.IsTrue(service.CompleteTask(saved));

            var newTask = service.GetTask().First(t => t.Id != 1);
            Assert.AreEqual(start.AddYears(1), newTask.Deadline);

            ctx.Dispose();
        }
    }
}
