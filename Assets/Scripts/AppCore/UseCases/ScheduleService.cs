using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using Domain.Entity;
using Domain.Enum;
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

        public void Setup() {}

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

        public List<UnitSchedule> GetSchedulesInDuration(ScheduleDuration duration)
        {
            var ret = new List<UnitSchedule>();

            foreach (var schedule in _scheduleRepo.GetAll())
            {
                if (schedule.Periodic is null)
                {
                    if (schedule.Duration.IsCollided(duration)) ret.Add(schedule);
                }
                else
                {
                    var index = 0;
                    var currentDuration = schedule.Duration;
                    while (currentDuration.StartTime.CompareTo(currentDuration.EndTime) <= 0)
                    {
                        var nextDuration = GetDurationByPeriodic(schedule.Duration, schedule.Periodic, index++);
                        if (nextDuration.IsCollided(duration))
                        {
                            ret.Add(new UnitSchedule(schedule.Id, schedule.Title, schedule.Description, nextDuration));
                        }
                        currentDuration = nextDuration;
                    }
                }
            }

            return ret;
        }

        public Dictionary<CCDateOnly, List<UnitSchedule>> GetSchedulesInDuration(CCDateOnly startDate, CCDateOnly endDate)
        {
            var ret = new Dictionary<CCDateOnly, List<UnitSchedule>>();

            for (var date = startDate; date.CompareTo(endDate) <= 0; date = date.AddDays(1))
            {
                ret.Add(date, GetSchedulesInDuration(new ScheduleDuration(date)));
            }
            
            return ret;
        }

        private ScheduleDuration GetDurationByPeriodic(ScheduleDuration duration, SchedulePeriodic periodic, int loop)
        {
            switch (periodic.PeriodicType)
            {
                case SchedulePeriodicType.EveryDay:
                    return new ScheduleDuration(duration.StartTime.AddDays(loop), duration.EndTime.AddDays(loop));
                case SchedulePeriodicType.EveryWeekday:
                {
                    var weekdays = 
                        Enum.GetValues(typeof(DayOfWeek)).AsValueEnumerable().Select(day => (DayOfWeek)day)
                            .ToDictionary(day => (int)day, day => (periodic.Span & (1 << (int)day)) != 0);

                    var next = (int)duration.StartTime.DayOfWeek;
                    var elapseDays = 0;
                    for (var i = 0; i < loop+1; elapseDays++)
                    {
                        next = (next + 1) % 7;
                        if (weekdays[next]) i++;
                    }
                    
                    return new ScheduleDuration(duration.StartTime.AddDays(elapseDays), duration.EndTime.AddDays(elapseDays));
                }
                case SchedulePeriodicType.EveryWeek:
                    return new ScheduleDuration(duration.StartTime.AddDays(7*loop), duration.EndTime.AddDays(7*loop));
                case SchedulePeriodicType.EveryMonth:
                    return new ScheduleDuration(duration.StartTime.AddMonths(loop), duration.EndTime.AddMonths(loop));
                case SchedulePeriodicType.EveryYear:
                    return new ScheduleDuration(duration.StartTime.AddYears(loop), duration.EndTime.AddYears(loop));
            }
            
            return duration;
        }
        
        public void Dispose()
        {
        }
    }
}