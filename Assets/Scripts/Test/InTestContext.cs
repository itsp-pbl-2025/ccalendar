#nullable enable
using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using AppCore.UseCases;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Test
{
    public class InTestContext
    {
        private class TestContext : IContext
        {
            private readonly LiteDBManager _liteDb;
            private readonly List<IService> _services = new();
            private readonly Dictionary<Type, Func<string, IService>> _serviceFactories = new();
            
            public TestContext()
            {
                _liteDb = new LiteDBManager();
            
                SetupServiceFactories();
            }
            
            public bool Ready => true;
        
            private void SetupServiceFactories()
            {
                _serviceFactories.Add(typeof(SampleService), name => new SampleService(ScheduleRepo, name));
                _serviceFactories.Add(typeof(ScheduleService), name => new ScheduleService(ScheduleRepo, name));
                _serviceFactories.Add(typeof(HolidayService), name => new HolidayService(this, name));
                _serviceFactories.Add(typeof(HistoryService), name => new HistoryService(HistoryRepo, name));
                _serviceFactories.Add(typeof(TaskService), name => new TaskService(TaskRepo, name));
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

                if (_serviceFactories.TryGetValue(typeof(T), out var factory))
                {
                    var service = factory(name);
                    _services.Add(service);
                    service.Setup();
                    if (service is T typedService) return typedService;
                    throw new InvalidOperationException($"ServiceFactory of type {typeof(T).Name} creates wrong service type.");
                }
                
                throw new ArgumentOutOfRangeException($"Service of type {typeof(T).Name} not found.");
            }
            
            private ScheduleRepository? _scheduleRepo;
            private TaskRepository? _taskRepo;
            public IScheduleRepository ScheduleRepo => _scheduleRepo ??= new ScheduleRepository(_liteDb.DB);
            public ITaskRepository TaskRepo => _taskRepo ??= new TaskRepository(_liteDb.DB);

            private HistoryRepository? _historyRepo;
            public IHistoryRepository HistoryRepo => _historyRepo ??= new HistoryRepository(_liteDb.DB);

            public void Dispose()
            {
                _liteDb.Dispose();
                foreach (var service in _services) service.Dispose();
            }
        }
        
        public static IContext Context => new TestContext();
    }
}