using System.Collections.Generic;
using Domain.Enum;

namespace Domain.Entity
{
    public record SchedulePeriodic(
        SchedulePeriodicType PeriodicType,
        int Span,
        IReadOnlyList<int> ExcludeIndices
    )
    {
        // ExcludeIndices を省略したい場合の重載コンストラクタ
        public SchedulePeriodic(
            SchedulePeriodicType periodicType,
            int span
        ) : this(periodicType, span, new List<int>())
        { }

        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
        public IReadOnlyList<int> ExcludeIndices { get; } = ExcludeIndices;
    }
}