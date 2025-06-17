using Domain.Entity;
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
                    StartTime = new CCDateTime(sc.Duration.StartTime),
                    EndTime = new CCDateTime(sc.Duration.EndTime),
                    IsAllDay = new CCDateTime(sc.Duration.IsAllDay),
                },
                Periodic = sc.Periodic is null
                    ? null
                    : new DSchedulePeriodic()
                    {
                        PeriodicType = sc.Periodic.PeriodicType,
                        Span = sc.Periodic.Span,
                    },
            };
        }

        public static Schedule ToDomain(this DSchedule sc)
        {
            ScheduleDuration duration =
                new ScheduleDuration(new CCDateTime(sc.Duration.StartTime), new CCDateTime(sc.Duration.EndTime), sc.Duration.IsAllDay);
            SchedulePeriodic periodic = sc.Periodic is null
                ? null
                : new SchedulePeriodic(sc.Periodic.PeriodicType, sc.Periodic.Span);
            return new Schedule(sc.Id, sc.Title, sc.Description, duration, periodic);
        }
    }
}