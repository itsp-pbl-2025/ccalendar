using Domain.Entity;
using System.Collections.Generic;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class ScheduleDao
    {
        public static DSchedule FromDomain(this Schedule sc)
        {
            return new DSchedule()
            {
                Id = sc.Id,
                Title = sc.Title,
                Description = sc.Description,
                Duration = new DScheduleDuration()
                {
                    StartTime = sc.Duration.StartTime.ToDateTime(),
                    EndTime = sc.Duration.EndTime.ToDateTime(),
                    IsAllDay = sc.Duration.IsAllDay,
                },
                Periodic = sc.Periodic is null
                    ? null
                    : new DSchedulePeriodic()
                    {
                        PeriodicType = sc.Periodic.PeriodicType,
                        Span = sc.Periodic.Span,
                        StartDate = sc.Periodic.StartDate.ToDateTime(),
                        EndDate = sc.Periodic.EndDate?.ToDateTime()
                        ExcludeIndices = sc.Periodic.ExcludeIndices,
                    },
            };
        }

        public static Schedule ToDomain(this DSchedule sc)
        {
            ScheduleDuration duration =
                new ScheduleDuration(new CCDateTime(sc.Duration.StartTime), new CCDateTime(sc.Duration.EndTime), sc.Duration.IsAllDay);
            SchedulePeriodic periodic = sc.Periodic is null
                ? null
                : new SchedulePeriodic(
                    sc.Periodic.PeriodicType,
                    sc.Periodic.Span,
                    new CCDateTime(sc.Periodic.StartDate).ToDateOnly(),
                    sc.Periodic.EndDate.HasValue ? new CCDateTime(sc.Periodic.EndDate.Value).ToDateOnly() : null,
                    sc.Periodic.ExcludeIndices ?? new List<int>()
                    );
            return new Schedule(sc.Id, sc.Title, sc.Description, duration, periodic);
        }
    }
}