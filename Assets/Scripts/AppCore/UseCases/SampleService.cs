using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;

namespace AppCore.UseCases
{
    public class SampleService : IService
    {
        private readonly IScheduleRepository _scheduleRepo;
        
        public string Name { get; }
        
        public SampleService(IScheduleRepository repo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _scheduleRepo = repo;
        }

        public void Setup() {}

        public ICollection<Schedule> GetSchedules()
        {
            return _scheduleRepo.GetAll();
        }

        public void Dispose()
        {
            
        }
    }
}