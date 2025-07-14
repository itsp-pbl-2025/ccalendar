using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
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
            // If the task is not periodic, we can simply update the task to completed
                CCTask completedTask = new CCTask(
                        Id: task.Id,
                        Title: task.Title,
                        Description: task.Description,
                        Priority: task.Priority,
                        Deadline: task.Deadline,
                        Duration: task.Duration,
                        IsCompleted: true
                    );
                    return UpdateTask(completedTask);
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
