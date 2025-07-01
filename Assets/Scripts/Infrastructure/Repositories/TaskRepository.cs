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
        private readonly ILiteCollection<DScheduleTask> _col;
        
        public TaskRepository(LiteDatabase db) => _col = db.GetCollection<DScheduleTask>("task");
        
        public ScheduleTask Insert(ScheduleTask scheduleTask)
        {
            var dtask = scheduleTask.FromDomain();
            dtask.Id = _col.Insert(dtask).AsInt32;
            return dtask.ToDomain();
        }

        public bool Update(ScheduleTask scheduleTask)
        {
            var dtask = scheduleTask.FromDomain();
            return _col.Update(dtask);
        }
        
        public bool InsertUpdate(ScheduleTask scheduleTask)
        {
            var dtask = scheduleTask.FromDomain();
            return _col.Upsert(dtask);
        }
        
        public bool Remove(ScheduleTask scheduleTask)
        {
            return _col.Delete(scheduleTask.Id);
        }
        
        public ICollection<ScheduleTask> GetAll()
        {
            var result = new List<ScheduleTask>();
            foreach (var task in _col.FindAll())
            {
                result.Add(task.ToDomain());
            }
            return result;
        }
    }
}