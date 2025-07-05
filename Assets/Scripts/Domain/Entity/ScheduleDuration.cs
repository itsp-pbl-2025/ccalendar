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
        public ScheduleDuration(CCDateOnly day = default)
            : this(new CCDateTime(day, new CCTimeOnly()), new CCDateTime(day, new CCTimeOnly(23, 59, 59)), true)
        {
            if (day.Year.Value != 0 && day.Month.Value != 0 && day.Day.Value != 0) return;
            
            // CCDateOnlyがデフォルトコンストラクタで宣言された場合、今日に変換する
            var today = CCDateOnly.Today;
            StartTime = new CCDateTime(today, new CCTimeOnly(0, 0, 0));
            EndTime = new CCDateTime(today, new CCTimeOnly(23, 59, 59));
        }
        
        public CCDateTime StartTime { get; }
        public CCDateTime EndTime { get; }
        public bool IsAllDay { get; }
    }
}