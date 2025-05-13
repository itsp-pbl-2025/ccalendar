using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface IScheduleRepository
    {
        public bool InsertUpdate(ISchedule schedule);
        public ICollection<ISchedule> GetAll();
    }
}