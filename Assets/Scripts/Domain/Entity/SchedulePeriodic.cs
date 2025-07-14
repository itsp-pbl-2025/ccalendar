using System.Collections.Immutable;
using Domain.Enum;

namespace Domain.Entity
{
    public record SchedulePeriodic(
        SchedulePeriodicType PeriodicType,
        int Span,
        ImmutableList<int> ExcludeIndices
    )
    {
        private static readonly ImmutableList<int> DefaultExcludeIndices = ImmutableList<int>.Empty;
        
        // ExcludeIndices を省略したい場合の重載コンストラクタ
        public SchedulePeriodic(
            SchedulePeriodicType periodicType,
            int span
        ) : this(periodicType, span, DefaultExcludeIndices)
        { }

        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
        public ImmutableList<int> ExcludeIndices { get; } = ExcludeIndices;
    }
}