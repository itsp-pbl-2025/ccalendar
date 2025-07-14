using System;
using System.Collections.Immutable;
using Domain.Enum;

namespace Infrastructure.Data.Schema
{
    public class DSchedulePeriodic
    {
        public SchedulePeriodicType PeriodicType { get; set; }
        public int Span { get; set; }
        
        public ImmutableList<int> ExcludeIndices { get; set; }
    }
}
