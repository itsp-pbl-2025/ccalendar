using System;

namespace Domain.Entity
{
    public record ScheduleDuration
    {
        /**
         * デフォルトコンストラクタ.
         * DAO経由でDScheduleから変換する場合以外は使わないこと.
         */
        public ScheduleDuration(CCDateTime startTime, CCDateTime endTime, bool isAllDay)
        {
            StartTime = startTime;
            EndTime = endTime;
            IsAllDay = isAllDay;
        }
        
        /**
         * 期間を指定する(終日でない)場合のコンストラクタ.
         */
        public ScheduleDuration(CCDateTime startTime, CCDateTime endTime)
            : this(startTime, endTime, false)
        {
            if (startTime.CompareTo(endTime) > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(endTime), "The end time must be later than the start time.");
            }
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
                throw new ArgumentOutOfRangeException(nameof(endDay), "The end day must be later than the start day.");
            }
            StartTime = new CCDateTime(day, new CCTimeOnly(0, 0, 0));
            EndTime = new CCDateTime(endDay, new CCTimeOnly(23, 59, 59));
            IsAllDay = true;
        }
        
        public CCDateTime StartTime { get; }
        public CCDateTime EndTime { get; }
        public bool IsAllDay { get; }

        public bool IsInSet(CCDateTime time)
        {
            return time >= StartTime && time <= EndTime;
        }

        public bool IsCollided(ScheduleDuration other)
        {
            return EndTime >= other.StartTime && StartTime <= other.EndTime;
        }
    }
}