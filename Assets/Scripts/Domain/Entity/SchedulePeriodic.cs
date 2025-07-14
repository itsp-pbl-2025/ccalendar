using System;
using System.Collections.Immutable;
using Domain.Enum;
using System.Linq;

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
        
        public virtual bool Equals(SchedulePeriodic? other) =>
            other is not null
            && PeriodicType == other.PeriodicType
            && Span == other.Span
            && ExcludeIndices.SequenceEqual(other.ExcludeIndices);

        public override int GetHashCode() =>
            HashCode.Combine(
                PeriodicType,
                Span,
                ExcludeIndices.Aggregate(0, (h, v) => HashCode.Combine(h, v))
            );

        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
        public ImmutableList<int> ExcludeIndices { get; } = ExcludeIndices;
    }
}