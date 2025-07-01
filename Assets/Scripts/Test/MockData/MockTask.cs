using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entity;
using Infrastructure.Data.DAO;
using Infrastructure.Data.Schema;

namespace Test.MockData
{
    public static class MockTask
    {
        public static List<DScheduleTask> GetMockTasksWithoutId()
        {
            List<ScheduleTask> tasks = new List<ScheduleTask>
            {
                new(0,
                    "High Priority Task",
                    "This is a high priority task with urgent deadline",
                    1,
                    DateTime.Today.AddDays(1)),
                new(0,
                    "Medium Priority Task",
                    "This is a medium priority task",
                    2,
                    DateTime.Today.AddDays(7)),
                new(0,
                    "Low Priority Task",
                    "This is a low priority task with flexible deadline",
                    3,
                    DateTime.Today.AddDays(30)),
                new(0, "", "", 0, DateTime.Today),
            };

            return tasks.Select(e => e.FromDomain()).ToList();
        }

        public static List<DScheduleTask> GetMockTasks()
        {
            List<ScheduleTask> tasks = new List<ScheduleTask>
            {
                new(1,
                    "High Priority Task",
                    "This is a high priority task with urgent deadline",
                    1,
                    DateTime.Today.AddDays(1)),
                new(2,
                    "Medium Priority Task",
                    "This is a medium priority task",
                    2,
                    DateTime.Today.AddDays(7)),
                new(3,
                    "Low Priority Task",
                    "This is a low priority task with flexible deadline",
                    3,
                    DateTime.Today.AddDays(30)),
                new(4, "", "", 0, DateTime.Today),
            };

            return tasks.Select(e => e.FromDomain()).ToList();
        }
    }
}