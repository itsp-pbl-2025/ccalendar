#nullable enable

namespace Domain.Entity
{
    public record Schedule(int Id, string Title, string Description, ScheduleDuration Duration, SchedulePeriodic? Periodic = null)
        : UnitSchedule(Id, Title, Description, Duration)
    {
        public SchedulePeriodic? Periodic { get; } = Periodic;
    }
}