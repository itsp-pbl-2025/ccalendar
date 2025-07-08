using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using Domain.Enum;
using ZLinq;

namespace AppCore.UseCases
{
    public class TaskService : IService
    {
        private readonly ITaskRepository _taskRepo;
        
        public string Name { get; }
        
        public void Setup() { }

        public TaskService(ITaskRepository repo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _taskRepo = repo;
        }

        public void CreateTask(CCTask task)
        {
            _taskRepo.Insert(task);
        }

        public bool CompleteTask(CCTask task)
        {
            if (task.Periodic is null)
            {
                // If the task is not periodic, we can simply update the task to completed
                CCTask completedTask = new CCTask(
                        Id: task.Id,
                        Title: task.Title,
                        Description: task.Description,
                        Priority: task.Priority,
                        Deadline: task.Deadline,
                        Duration: task.Duration,
                        Periodic: task.Periodic,
                        IsCompleted: true
                    );
                return UpdateTask(completedTask);
            }
            else
            {
                CCDateTime newDeadline;
                // If the task is periodic, we need to create a new instance of the task with updated properties
                switch (task.Periodic.PeriodicType)
                {
                    case TaskPeriodicType.EveryWeekday:
                        // This logic assumes we advance by 'Span' number of weekdays.
                        var currentDate = task.Deadline;
                        var weekdaysToAdd = task.Periodic.Span;
                        while (weekdaysToAdd > 0)
                        {
                            currentDate = currentDate.AddDays(1);
                            if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                weekdaysToAdd--;
                            }
                        }
                        newDeadline = currentDate;
                        break;
                    case TaskPeriodicType.EveryWeek:
                        newDeadline = task.Deadline.AddDays(task.Periodic.Span * 7);
                        break;
                    case TaskPeriodicType.EveryMonth:
                        newDeadline = task.Deadline.AddMonths(task.Periodic.Span);
                        break;
                    case TaskPeriodicType.EveryYear:
                        newDeadline = task.Deadline.AddYears(task.Periodic.Span);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(task.Periodic.PeriodicType), $"Unsupported periodic type: {task.Periodic.PeriodicType}");
                }
                
                CCTask completedTask = new CCTask(
                    Id: task.Id,
                    Title: task.Title,
                    Description: task.Description,
                    Priority: task.Priority,
                    Deadline: newDeadline,
                    Duration: task.Duration,
                    Periodic: task.Periodic,
                    IsCompleted: true
                );
                
                CCTask updatedTask = new CCTask(
                    Id: task.Id,
                    Title: task.Title,
                    Description: task.Description,
                    Priority: task.Priority,
                    Deadline: newDeadline,
                    Duration: task.Duration,
                    Periodic: task.Periodic,
                    IsCompleted: false
                );
                CreateTask(updatedTask);
                return UpdateTask(updatedTask);
            }
        }

        public bool UpdateTask(CCTask task)
        {
            return _taskRepo.Update(task);
        }
        
        public bool DeleteTask(CCTask task)
        {
            return _taskRepo.Remove(task);
        }
        
        public List<CCTask> GetTask()
        {
            return _taskRepo.GetAll().AsValueEnumerable().ToList();
        }
        
        public void Dispose()
        {
        }
    }
}
