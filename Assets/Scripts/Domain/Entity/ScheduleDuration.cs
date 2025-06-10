using System;

namespace Domain.Entity
{
    public record ScheduleDuration(DateTime StartTime, DateTime EndTime, bool IsAllDay)
    {
        public DateTime StartTime { get; } = StartTime;
        public DateTime EndTime { get; } = EndTime;
        public bool IsAllDay { get; } = IsAllDay;
    }
}