using Domain.Enum;

namespace Domain.Entity
{
    /*
    public record SchedulePeriodic(SchedulePeriodicType PeriodicType, int Span)
    {
        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;
    }
    */
    public record SchedulePeriodic(
        SchedulePeriodicType PeriodicType,
        int Span,
        CCDateOnly? StartDate = null,
        CCDateOnly? EndDate = null)
    {
        public SchedulePeriodicType PeriodicType { get; } = PeriodicType;
        public int Span { get; } = Span;

        /// <summary>繰り返しの開始日（nullならスケジュール本体のStartTimeを使用）</summary>
        public CCDateOnly? StartDate { get; } = StartDate;

        /// <summary>繰り返しの終了日（nullなら無期限）</summary>
        public CCDateOnly? EndDate { get; } = EndDate;

        /// <summary>指定された日付が繰り返し範囲内にあるかどうか</summary>
        public bool IsInRange(CCDateOnly date)
        {
            var afterStart = !StartDate.HasValue || date.CompareTo(StartDate.Value) >= 0;
            var beforeEnd = !EndDate.HasValue || date.CompareTo(EndDate.Value) <= 0;
            return afterStart && beforeEnd;
        }
    }

}