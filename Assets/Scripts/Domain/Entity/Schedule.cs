using Domain.Enum;

namespace Domain.Entity
{
    public interface ISchedule
    {
        public int Id { get; }
        public ScheduleType Type { get; }
        public string Title { get; }
    }

    public record DateSchedule(int Id, string Title) : ISchedule
    {
        public int Id { get; } = Id;
        
        public ScheduleType Type => ScheduleType.Date;
        public string Title { get; } = Title;
    }

    public record DurationSchedule(int Id, string Title) : ISchedule
    {
        public int Id { get; } = Id;
        
        public ScheduleType Type => ScheduleType.Duration;
        public string Title { get; } = Title;
    }
}