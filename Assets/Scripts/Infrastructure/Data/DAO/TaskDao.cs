using Domain.Entity;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class TaskDao
    {
        public static DTask FromDomain(this Task ts)
        {
            return new DTask()
            {
                Id = ts.Id,
                Title = ts.Title,
                Description = ts.Description,
                Priority = ts.Priority,
                Deadline = ts.Deadline,
            };
        }

        public static Task ToDomain(this DTask ts)
        {
            return new Task(ts.Id, ts.Title, ts.Description, ts.Priority, ts.Deadline);
        }
    }
}