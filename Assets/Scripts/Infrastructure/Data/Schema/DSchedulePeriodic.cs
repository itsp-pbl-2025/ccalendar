using System;
using System.Collections.Generic;
using Domain.Enum;

namespace Infrastructure.Data.Schema
{
    public class DSchedulePeriodic
    {
        public SchedulePeriodicType PeriodicType { get; set; }
        public int Span { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        
        public IReadOnlyList<int> ExcludeIndices { get; set; } = new List<int>();
    }
}
