using Domain.Enum;

namespace Domain.Entity
{
    public record SchedulePeriodic(SchedulePeriodicType PeriodicType, int Span)
    {
        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
    }
}