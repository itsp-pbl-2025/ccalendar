using Domain.Enum;

namespace Domain.Entity
{
    public interface ISchedule
    {
        public int Id { get; }
        public ScheduleType Type { get; }
        public string Title { get; }
    }

    public class DateSchedule : ISchedule
    {
        public DateSchedule(int id, string title)
        {
            Id = id;
            Title = title;
        }
        
        public int Id { get; }
        
        public ScheduleType Type => ScheduleType.Date;
        public string Title { get; }
    }

    public class DurationSchedule : ISchedule
    {
        public DurationSchedule(int id, string title)
        {
            Id = id;
            Title = title;
        }
        
        public int Id { get; }
        
        public ScheduleType Type => ScheduleType.Duration;
        public string Title { get; }
    }
}