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
        public static List<DCCTask> GetMockTasksWithoutId()
        {
            List<CCTask> tasks = new List<CCTask>
            {
                new(0,
                    "High Priority Task",
                    "This is a high priority task with urgent deadline",
                    1,
                    CCDateTime.Today.AddDays(1)),
                new(0,
                    "Medium Priority Task",
                    "This is a medium priority task",
                    2,
                    CCDateTime.Today.AddDays(7)),
                new(0,
                    "Low Priority Task",
                    "This is a low priority task with flexible deadline",
                    3,
                    CCDateTime.Today.AddDays(30)),
                new(0, "", "", 0, CCDateTime.Today),
            };

            return tasks.Select(e => e.FromDomain()).ToList();
        }

        public static List<DCCTask> GetMockTasks()
        {
            List<CCTask> tasks = new List<CCTask>
            {
                new(1,
                    "High Priority Task",
                    "This is a high priority task with urgent deadline",
                    1,
                    CCDateTime.Today.AddDays(1)),
                new(2,
                    "Medium Priority Task",
                    "This is a medium priority task",
                    2,
                    CCDateTime.Today.AddDays(7)),
                new(3,
                    "Low Priority Task",
                    "This is a low priority task with flexible deadline",
                    3,
                    CCDateTime.Today.AddDays(30)),
                new(4, "", "", 0, CCDateTime.Today),
            };

            return tasks.Select(e => e.FromDomain()).ToList();
        }
    }
}