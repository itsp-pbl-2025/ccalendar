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
        
        public TaskService(ITaskRepository repo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _taskRepo = repo;
        }

        public void CreateTask(CCTask task)
        {
            _taskRepo.Insert(task);
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
