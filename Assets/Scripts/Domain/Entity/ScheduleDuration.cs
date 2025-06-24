using System;

namespace Domain.Entity
{
    public record ScheduleDuration
    {
        /**
         * デフォルトコンストラクタ.
         * DAO経由でDScheduleから変換する場合以外は使わないこと.
         */
        public ScheduleDuration(DateTime StartTime, DateTime EndTime, bool IsAllDay)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.IsAllDay = IsAllDay;
        }
        
        /**
         * 期間を指定する(終日でない)場合のコンストラクタ.
         */
        public ScheduleDuration(DateTime StartTime, DateTime EndTime)
            : this(StartTime, EndTime, false)
        {
        }
        
        /**
         * 終日の場合のコンストラクタ.
         */
        public ScheduleDuration()
            : this(DateTime.Today, DateTime.Today.AddDays(1).AddMinutes(-1), true)
        {
        }
        
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public bool IsAllDay { get; }
    }
}