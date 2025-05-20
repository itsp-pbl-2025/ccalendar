using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface IScheduleRepository
    {
        public ISchedule Insert(ISchedule schedule);
        public bool Update(ISchedule schedule);
        public bool InsertUpdate(ISchedule schedule);
        public bool Remove(ISchedule schedule);
        public ICollection<ISchedule> GetAll();
    }
}