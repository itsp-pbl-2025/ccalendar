using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using ZLinq;

namespace AppCore.UseCases
{
    public class Task2ScheduleService : IService
    {
        
        public string Name { get; }
        private readonly IContext _context;
        
        public Task2ScheduleService(IContext context,string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _context = context;
        }
        
        public void Dispose()
        {
        }
    }
}