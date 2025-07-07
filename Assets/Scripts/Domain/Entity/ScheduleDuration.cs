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
        public ScheduleDuration(CCDateOnly day)
        {
            StartTime = new CCDateTime(day, new CCTimeOnly(0, 0, 0));
            EndTime = new CCDateTime(day, new CCTimeOnly(23, 59, 59));
            IsAllDay = true;
        }
        
        /**
         * 終日(複数日)の場合のコンストラクタ.
         */
        public ScheduleDuration(CCDateOnly day, CCDateOnly endDay)
        {
            if (day.CompareTo(endDay) > 0)
            {
                (day, endDay) = (endDay, day);
            }
            StartTime = new CCDateTime(day, new CCTimeOnly(0, 0, 0));
            EndTime = new CCDateTime(endDay, new CCTimeOnly(23, 59, 59));
            IsAllDay = true;
        }
        
        public CCDateTime StartTime { get; }
        public CCDateTime EndTime { get; }
        public bool IsAllDay { get; }
    }
}