using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using Infrastructure.Data.DAO;
using Infrastructure.Data.Schema;
using LiteDB;

namespace Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ILiteCollection<DSchedule> _col;

        public ScheduleRepository(LiteDatabase db) => _col = db.GetCollection<DSchedule>("schedule");

        public Schedule Insert(Schedule schedule)
        {
            var dSchedule = schedule.FromDomain();
            dSchedule.Id = _col.Insert(dSchedule).AsInt32;
            return dSchedule.ToDomain();
        }
        
        public bool Update(Schedule schedule)
        {
            var dSchedule = schedule.FromDomain();
            return _col.Update(dSchedule);
        }
        
        public bool InsertUpdate(Schedule schedule)
        {
            var dSchedule = schedule.FromDomain();
            return _col.Upsert(dSchedule);
        }
        
        public bool Remove(Schedule schedule)
        {
            return _col.Delete(schedule.Id);
        }
        
        public ICollection<Schedule> GetAll()
        {
            var result = new List<Schedule>();
            foreach (var schedule in _col.FindAll())
            {
                result.Add(schedule.ToDomain());
            }
            return result;
        }
    }
}