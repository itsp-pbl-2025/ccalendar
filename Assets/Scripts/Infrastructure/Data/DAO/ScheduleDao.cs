using System;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class ScheduleDao
    {
        public static DSchedule FromDomain(ISchedule sc)
        {
            return sc.Type switch
            {
                ScheduleType.Date => new DSchedule()
                {
                    Id = sc.Id,
                    Type = ScheduleType.Date,
                    Title = sc.Title
                },
                ScheduleType.Duration => new DSchedule()
                {
                    Id = sc.Id,
                    Type = ScheduleType.Duration,
                    Title = sc.Title
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static ISchedule FromDomain(DSchedule sc)
        {
            return sc.Type switch
            {
                ScheduleType.Date => new DateSchedule(sc.Id, sc.Title),
                ScheduleType.Duration => new DurationSchedule(sc.Id, sc.Title),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}