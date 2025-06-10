using System;

namespace Infrastructure.Data.Schema
{
    public class DScheduleDuration
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
    }
}