using System;
using System.Collections.Generic;
using Domain.Enum;
using System.Linq;

namespace Domain.Entity
{
    public record SchedulePeriodic(
        SchedulePeriodicType PeriodicType,
        int Span,
        IReadOnlyList<int> ExcludeIndices
    )
    {
        private static readonly IReadOnlyList<int> DefaultExcludeIndices = new List<int>();
        
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

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(PeriodicType);
            hash.Add(Span);
            foreach (var i in ExcludeIndices)
            {
                hash.Add(i);
            }
            return hash.ToHashCode();
        }

        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
        public IReadOnlyList<int> ExcludeIndices { get; } = ExcludeIndices;
    }
}