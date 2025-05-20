using System.Collections.Generic;
using System.Linq;
using AppCore.Interfaces;
using Domain.Entity;

namespace AppCore.UseCases
{
    public class ScheduleService : IService
    {
        private readonly IScheduleRepository _scheduleRepo;
        
        public string Name { get; }
        
        public ScheduleService(IScheduleRepository repo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _scheduleRepo = repo;
        }

        public void CreateSchedule(ISchedule schedule)
        {
            _scheduleRepo.Insert(schedule);
        }
        
        public bool UpdateSchedule(ISchedule schedule)
        {
            return _scheduleRepo.Update(schedule);
        }
        
        public bool DeleteSchedule(ISchedule schedule)
        {
            return _scheduleRepo.Remove(schedule);
        }
        
        public List<ISchedule> GetSchedules()
        {
            return _scheduleRepo.GetAll().ToList();
        }
        
        public void Dispose()
        {
        }
    }
}