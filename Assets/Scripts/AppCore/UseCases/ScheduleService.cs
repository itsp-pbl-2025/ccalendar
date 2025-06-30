using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using ZLinq;

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

        public void CreateSchedule(Schedule schedule)
        {
            _scheduleRepo.Insert(schedule);
        }
        
        public bool UpdateSchedule(Schedule schedule)
        {
            return _scheduleRepo.Update(schedule);
        }
        
        public bool DeleteSchedule(Schedule schedule)
        {
            return _scheduleRepo.Remove(schedule);
        }
        
        public List<Schedule> GetSchedules()
        {
            return _scheduleRepo.GetAll().AsValueEnumerable().ToList();
        }
        
        public void Dispose()
        {
        }
    }
}