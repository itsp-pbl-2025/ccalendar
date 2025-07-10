using System;
using Domain.Enum;

namespace Infrastructure.Data.Schema
{
    public class DTaskPeriodic
    {
        public TaskPeriodicType PeriodicType { get; set; }
        public int Span { get; set; }
    }
}
