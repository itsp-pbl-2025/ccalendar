using System;
using System.Collections.Generic;
using Domain.Enum;

namespace Infrastructure.Data.Schema
{
    public class DSchedulePeriodic
    {
        public SchedulePeriodicType PeriodicType { get; set; }
        public int Span { get; set; }
        
        public IReadOnlyList<int> ExcludeIndices { get; set; }
    }
}
