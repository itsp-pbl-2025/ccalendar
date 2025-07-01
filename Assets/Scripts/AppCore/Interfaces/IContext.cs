using System;
using AppCore.UseCases;

namespace AppCore.Interfaces
{
    public interface IContext : IDisposable
    {
        public bool Ready { get; }
        
        public T GetService<T>(string name = "") where T : IService;
        public IScheduleRepository ScheduleRepo { get; }
        public ITaskRepository TaskRepo { get; }
        public IHistoryRepository HistoryRepo { get; }
    }
}