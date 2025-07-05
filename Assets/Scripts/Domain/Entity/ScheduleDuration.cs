using System;

namespace Domain.Entity
{
    public record ScheduleDuration
    {
        /**
         * デフォルトコンストラクタ.
         * DAO経由でDScheduleから変換する場合以外は使わないこと.
         */
        public ScheduleDuration(CCDateTime StartTime, CCDateTime EndTime, bool IsAllDay)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.IsAllDay = IsAllDay;
        }
        
        /**
         * 期間を指定する(終日でない)場合のコンストラクタ.
         */
        public ScheduleDuration(CCDateTime StartTime, CCDateTime EndTime)
            : this(StartTime, EndTime, false)
        {
        }
        
        /**
         * 終日の場合のコンストラクタ.
         */
        public ScheduleDuration()
            : this(new CCDateTime(DateTime.Today), new CCDateTime(DateTime.Today.AddDays(1).AddMinutes(-1)), true)
        {
        }
        
        public CCDateTime StartTime { get; }
        public CCDateTime EndTime { get; }
        public bool IsAllDay { get; }

        public bool IsCollided(ScheduleDuration other)
        {
            return EndTime >= other.StartTime && StartTime <= other.EndTime;
        }
    }
}