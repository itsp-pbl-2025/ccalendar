using System;
using AppCore.UseCases;

namespace AppCore.Interfaces
{
    public interface IContext : IDisposable
    {
        public T GetService<T>(string name = "") where T : IService;
        public IScheduleRepository ScheduleRepo { get; }
    }
}