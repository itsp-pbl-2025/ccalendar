using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using AppCore.Utilities;
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
        
        public Schedule FindSchedule(int id)
        {
            return _scheduleRepo.Get(id);
        }
        
        public bool UpdateSchedule(Schedule schedule)
        {
            return _scheduleRepo.Update(schedule);
        }
        
        public bool DeleteSchedule(Schedule schedule)
        {
            return _scheduleRepo.Remove(schedule);
        }

        public bool ModifyScheduleAt(int scheduleId, int index, Schedule schedule)
        {
            var (id, title, description, duration, periodic) = _scheduleRepo.Get(scheduleId);
            if (periodic is null) return false;
            if (periodic.ExcludeIndices.AsValueEnumerable().Contains(index)) return false;
            
            var indices = new List<int>(periodic.ExcludeIndices) { index };

            UpdateSchedule(new Schedule(id, title, description, duration,
                new SchedulePeriodic(periodic.PeriodicType, periodic.Span, indices, periodic.StartDate, periodic.EndDate)));
            
            CreateSchedule(new Schedule(0, schedule.Title, schedule.Description, schedule.Duration));
            
            return true;
        }

        public bool ModifyScheduleForward(int scheduleId, CCDateOnly modifyAt, Schedule schedule)
        {
            var (id, title, description, duration, periodic) = _scheduleRepo.Get(scheduleId);
            if (periodic is null) return false;
            if (periodic.EndDate is not null && periodic.EndDate.Value.CompareTo(modifyAt) < 0) return false;
            
            UpdateSchedule(new Schedule(id, title, description, duration,
                new SchedulePeriodic(periodic.PeriodicType, periodic.Span, periodic.ExcludeIndices, periodic.StartDate, modifyAt.AddDays(-1))));
            
            var sPeriodic = schedule.Periodic;
            var newPeriodic = sPeriodic is null ? null : new SchedulePeriodic(sPeriodic.PeriodicType, sPeriodic.Span, sPeriodic.ExcludeIndices, modifyAt, sPeriodic.EndDate);
            CreateSchedule(new Schedule(0, schedule.Title, schedule.Description, schedule.Duration, newPeriodic));

            return true;
        }
        
        public List<Schedule> GetSchedules()
        {
            return _scheduleRepo.GetAll().AsValueEnumerable().ToList();
        }
        //キャッシュ
        private readonly Dictionary<int, SortedDictionary<int, ScheduleDuration>> _durationCache = new();

        public List<UnitSchedule> GetSchedulesInDuration(ScheduleDuration duration)
        {
            var ret = new List<UnitSchedule>();
            
            foreach (var schedule in _scheduleRepo.GetAll())
            {
                if (schedule.Periodic is null)
                {
                    if (duration.IsCollided(schedule.Duration)) ret.Add(schedule);
                }
                else
                {
                    
                    //周期的スケジュールの側の期限を終日で設定
                    CCDateTime? periodicEnd = schedule.Periodic.EndDate is null
                        ? (CCDateTime?)null
                        : new CCDateTime(
                            schedule.Periodic.EndDate.Value.Year .Value,
                            schedule.Periodic.EndDate.Value.Month.Value,
                            schedule.Periodic.EndDate.Value.Day  .Value,
                            23, 59, 59);
                    //スケジュールと周期の期限を比較
                    CCDateTime loopLimit = (periodicEnd is not null &&
                                            periodicEnd.Value.CompareTo(duration.EndTime) < 0)
                        ? periodicEnd.Value
                        : duration.EndTime;
                    
                    var index = 0;
                    while (true)
                    {
                        
                        //キャッシュに保存されているか確認
                        if (!_durationCache.TryGetValue(schedule.Id, out var inner))
                        {
                            inner = new SortedDictionary<int, ScheduleDuration>();
                            _durationCache[schedule.Id] = inner;
                        }
                        if (!inner.TryGetValue(index, out var currentDuration))
                        {
                            currentDuration = GetDurationByPeriodic(
                                schedule.Duration, schedule.Periodic, index);
                            inner[index] = currentDuration;
                        }
                        
                        //範囲外なら終了
                        if (currentDuration.StartTime.CompareTo(loopLimit) > 0) break;

                        //除外インデックスをスキップ
                        if (ContainsIndex(schedule.Periodic.ExcludeIndices, index))
                        {
                            index++;
                            continue;
                        }
                        
                        
                        if (currentDuration.IsCollided(duration))
                        {
                            ret.Add(new UnitSchedule(schedule.Id, schedule.Title, schedule.Description, currentDuration));
                        }
                        index++;
                        continue;

                        //ExcludeIndicesと一致するか判定
                        static bool ContainsIndex(IReadOnlyList<int> list, int value)
                        {
                            for (var i = 0; i < list.Count; i++)
                                if (i == value) return true;
                            return false;
                        }
                    }
                }
            }

            return ret;
        }
        
        private ScheduleDuration GetDurationByPeriodic(ScheduleDuration duration, SchedulePeriodic periodic, int loop)
        {
            switch (periodic.PeriodicType)
            {
                case SchedulePeriodicType.EveryDay:
                    return new ScheduleDuration(duration.StartTime.AddDays(periodic.Span*loop), duration.EndTime.AddDays(periodic.Span*loop), loop);
                case SchedulePeriodicType.EveryWeek:
                {
                    var weekdays = 
                        Enum.GetValues(typeof(DayOfWeek)).AsValueEnumerable().Select(day => (DayOfWeek)day)
                            .ToDictionary(day => (int)day, day => (periodic.Span & (1 << (int)day)) != 0);

                    // TODO: ループ処理が正当かどうかを確かめる
                    var next = (int)duration.StartTime.DayOfWeek;
                    if (loop is 0 && weekdays[next])
                    {
                        return duration;
                    }
                    
                    var elapseDays = 0;
                    for (var i = 0; i < loop+1; elapseDays++)
                    {
                        next = (next + 1) % 7;
                        if (weekdays[next]) i++;
                    }
                    
                    return new ScheduleDuration(duration.StartTime.AddDays(elapseDays), duration.EndTime.AddDays(elapseDays), loop);
                }
                case SchedulePeriodicType.EveryMonth:
                {
                    if (periodic.Span < 100)
                    {
                        var startTime = duration.StartTime.ToTimeOnly();
                        var startDate = duration.StartTime.ToDateOnly();
                        var scheduleSpan = (duration.EndTime - duration.StartTime).ToTimeSpan();

                        if (startDate.Day.Value < periodic.Span) loop++;
                        
                        var stepStartDate = new CCDateOnly(startDate.Year.Value, startDate.Month.Value, periodic.Span);
                        var startDateTime = new CCDateTime(stepStartDate.AddMonths(loop), startTime);
                        var endDate = new CCDateTime(startDateTime.ToDateTime() + scheduleSpan).ToDateOnly();
                        return new ScheduleDuration(startDateTime, new CCDateTime(endDate, startTime), loop);
                    }
                    else
                    {
                        var startTime = duration.StartTime.ToTimeOnly();
                        var startDate = duration.StartTime.ToDateOnly();
                        var scheduleSpan = (duration.EndTime - duration.StartTime).ToTimeSpan();
                        
                        var index = periodic.Span / 100;
                        var day = (DayOfWeek)(periodic.Span % 100);
                        
                        if (DateTimeExtensions.GetIndexedWeekDay(startDate.Year.Value, startDate.Month.Value, index, day).CompareTo(startDate) > 0) loop++;

                        var stepMonth = startDate.AddMonths(loop);
                        var stepStartDate = DateTimeExtensions.GetIndexedWeekDay(stepMonth.Year.Value, stepMonth.Month.Value, index, day);
                        var startDateTime = new CCDateTime(stepStartDate, startTime);
                        var endDate = new CCDateTime(startDateTime.ToDateTime() + scheduleSpan).ToDateOnly();
                        return new ScheduleDuration(startDateTime, new CCDateTime(endDate, startTime), loop);
                    }
                }
                case SchedulePeriodicType.EveryYear:
                    return new ScheduleDuration(duration.StartTime.AddYears(loop), duration.EndTime.AddYears(loop), loop);
            }
            
            return duration;
        }
        
        public void Dispose()
        {
        }
    }
}