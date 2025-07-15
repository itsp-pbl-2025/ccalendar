using System;
using System.Collections.Generic;
using Domain.Enum;
using ZLinq;

namespace Domain.Entity
{
    public record SchedulePeriodic(
        SchedulePeriodicType PeriodicType,
        int Span,
        IReadOnlyList<int> ExcludeIndices,
        CCDateOnly StartDate,
        CCDateOnly? EndDate = null
        )
    {
        private static readonly IReadOnlyList<int> DefaultExcludeIndices = new List<int>();
        
        // ExcludeIndices を省略したい場合の重載コンストラクタ
        public SchedulePeriodic(
            SchedulePeriodicType periodicType,
            int Span,
            CCDateOnly StartDate,
            CCDateOnly? EndDate = null
        ) : this(periodicType, Span, DefaultExcludeIndices, StartDate, EndDate)
        { }
        
        public virtual bool Equals(SchedulePeriodic? other) =>
            other is not null
            && PeriodicType == other.PeriodicType
            && Span == other.Span
            && ExcludeIndices.AsValueEnumerable().SequenceEqual(other.ExcludeIndices.AsValueEnumerable());

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
        public int Span { get; } = Span > 0 
            ? Span 
            : throw new ArgumentOutOfRangeException(nameof(Span), "Spanは1以上の整数です。");

        /// <summary>繰り返しの開始日</summary>
        public CCDateOnly StartDate { get; } = StartDate;

        /// <summary>繰り返しの終了日（nullなら無期限）</summary>
        public CCDateOnly? EndDate { get; } = EndDate;
        
        public IReadOnlyList<int> ExcludeIndices { get; } = ExcludeIndices;

        /// <summary>指定された日付が繰り返し範囲内にあるかどうか</summary>
        public bool IsInRange(CCDateOnly date)
        {
            var afterStart = date.CompareTo(StartDate) >= 0;
            var beforeEnd = !EndDate.HasValue || date.CompareTo(EndDate.Value) <= 0;
            return afterStart && beforeEnd;
        }
    }
}