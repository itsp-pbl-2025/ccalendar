#nullable enable

namespace Domain.Entity
{
    public record Schedule(int Id, string Title, string Description, ScheduleDuration Duration, SchedulePeriodic? Periodic = null)
    {
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public ScheduleDuration Duration { get; } = Duration;
        public SchedulePeriodic? Periodic { get; } = Periodic;
    }
}