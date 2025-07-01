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
        private readonly ILiteCollection<DCCTask> _col;
        
        public TaskRepository(LiteDatabase db) => _col = db.GetCollection<DCCTask>("task");
        
        public CCTask Insert(CCTask ccTask)
        {
            var dtask = ccTask.FromDomain();
            dtask.Id = _col.Insert(dtask).AsInt32;
            return dtask.ToDomain();
        }

        public bool Update(CCTask ccTask)
        {
            var dtask = ccTask.FromDomain();
            return _col.Update(dtask);
        }
        
        public bool InsertUpdate(CCTask ccTask)
        {
            var dtask = ccTask.FromDomain();
            return _col.Upsert(dtask);
        }
        
        public bool Remove(CCTask ccTask)
        {
            return _col.Delete(ccTask.Id);
        }
        
        public ICollection<CCTask> GetAll()
        {
            var result = new List<CCTask>();
            foreach (var task in _col.FindAll())
            {
                result.Add(task.ToDomain());
            }
            return result;
        }
    }
}