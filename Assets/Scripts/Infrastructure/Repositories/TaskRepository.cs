using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using Infrastructure.Data.DAO;
using Infrastructure.Data.Schema;
using LiteDB;

namespace Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ILiteCollection<DTask> _col;
        
        public TaskRepository(LiteDatabase db) => _col = db.GetCollection<DTask>("task");
        
        public Task Insert(Task task)
        {
            var dtask = task.FromDomain();
            dtask.Id = _col.Insert(dtask).AsInt32;
            return dtask.ToDomain();
        }

        public bool Update(Task task)
        {
            var dtask = task.FromDomain();
            return _col.Update(dtask);
        }
        
        public bool InsertUpdate(Task task)
        {
            var dtask = task.FromDomain();
            return _col.Upsert(dtask);
        }
        
        public bool Remove(Task task)
        {
            return _col.Delete(task.Id);
        }
        
        public ICollection<Task> GetAll()
        {
            var result = new List<Task>();
            foreach (var task in _col.FindAll())
            {
                result.Add(task.ToDomain());
            }
            return result;
        }
    }
}