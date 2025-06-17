using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface IScheduleRepository
    {
        public Schedule Insert(Schedule schedule);
        public bool Update(Schedule schedule);
        public bool InsertUpdate(Schedule schedule);
        public bool Remove(Schedule schedule);
        public ICollection<Schedule> GetAll();
    }
}