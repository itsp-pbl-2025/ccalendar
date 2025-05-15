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

        public bool InsertUpdate(ISchedule schedule)
        {
            var dSchedule = schedule.FromDomain();
            return _col.Upsert(dSchedule);
        }
        
        public ICollection<ISchedule> GetAll()
        {
            var result = new List<ISchedule>();
            foreach (var schedule in _col.FindAll())
            {
                result.Add(schedule.FromDomain());
            }
            return result;
        }
    }
}