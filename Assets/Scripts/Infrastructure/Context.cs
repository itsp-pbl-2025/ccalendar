#nullable enable
using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using AppCore.UseCases;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure
{
    public class Context : IContext
    {
        private readonly LiteDBManager _liteDb;
        private readonly List<IService> _services = new();

        public Context(string dbPath)
        {
            _liteDb = new LiteDBManager(dbPath);
            
            SetupServices();
        }
        
        private void SetupServices()
        {
            _services.Add(new SampleService(ScheduleRepo));
            _services.Add(new ScheduleService(ScheduleRepo));
            _services.Add(new HolidayService(ScheduleRepo));
            _services.Add(new HistoryService(HistoryRepo));
        }

        public T GetService<T>(string name = "") where T : IService
        {
            var findFirst = string.IsNullOrEmpty(name);
            foreach (var service in _services)
            {
                if (service is T tService && (findFirst || service.Name == name))
                {
                    return tService;
                }
            }

            throw new ArgumentOutOfRangeException($"{typeof(T).Name} {name} not found");
        }
        
        private ScheduleRepository? _scheduleRepo;
        public IScheduleRepository ScheduleRepo => _scheduleRepo ??= new ScheduleRepository(_liteDb.DB);
        
        private HistoryRepository? _historyRepo;
        public IHistoryRepository HistoryRepo => _historyRepo ??= new HistoryRepository(_liteDb.DB);
        
        public void Dispose()
        {
            _liteDb.Dispose();
            foreach (var service in _services) service.Dispose();
        }
    }
}