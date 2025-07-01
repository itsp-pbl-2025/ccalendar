using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface ITaskRepository
    {
        public ScheduleTask Insert(ScheduleTask scheduleTask);
        public bool Update(ScheduleTask scheduleTask);
        public bool InsertUpdate(ScheduleTask scheduleTask);
        public bool Remove(ScheduleTask scheduleTask);
        public ICollection<ScheduleTask> GetAll();
    }
}
