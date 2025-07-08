#nullable enable
using Domain.Enum;

namespace Domain.Entity
{
    public record TaskPeriodic(TaskPeriodicType PeriodicType, int Span)
    {
        public TaskPeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
    }
}