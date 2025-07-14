using Domain.Entity;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class TaskDao
    {
        public static DCCTask FromDomain(this CCTask ts)
        {
            return new DCCTask()
            {
                Id = ts.Id,
                Title = ts.Title,
                Description = ts.Description,
                Priority = ts.Priority,
                Deadline = ts.Deadline.ToDateTime(),
                Duration = ts.Duration.ToTimeSpan(),
                Periodic = ts.Periodic is null
                    ? null
                    : new DTaskPeriodic()
                    {
                        PeriodicType = ts.Periodic.PeriodicType,
                        Span = ts.Periodic.Span,
                    },
                IsCompleted = ts.IsCompleted
            };
        }

        public static CCTask ToDomain(this DCCTask ts)
        {
            CCDateTime deadline = new CCDateTime(ts.Deadline);
            return new CCTask(
                ts.Id, 
                ts.Title, 
                ts.Description, 
                ts.Priority, 
                deadline, 
                CCTimeSpan.FromTimeSpan(ts.Duration), 
                ts.Periodic is null 
                    ? null 
                    : new TaskPeriodic(ts.Periodic.PeriodicType, ts.Periodic.Span), 
                ts.IsCompleted
            );
        }
    }
}