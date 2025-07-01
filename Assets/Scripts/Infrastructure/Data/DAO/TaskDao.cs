using Domain.Entity;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class TaskDao
    {
        public static DScheduleTask FromDomain(this ScheduleTask ts)
        {
            return new DScheduleTask()
            {
                Id = ts.Id,
                Title = ts.Title,
                Description = ts.Description,
                Priority = ts.Priority,
                Deadline = ts.Deadline,
            };
        }

        public static ScheduleTask ToDomain(this DScheduleTask ts)
        {
            return new ScheduleTask(ts.Id, ts.Title, ts.Description, ts.Priority, ts.Deadline);
        }
    }
}