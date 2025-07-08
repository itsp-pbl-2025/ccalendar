#nullable enable
using System;
using Domain.Enum;

namespace Domain.Entity
{
    public record CCTask(int Id, string Title, string Description, int Priority, DateTime Deadline, CCTimeSpan Duration, TaskPeriodic? Periodic = null)
    {
        private static readonly CCTimeSpan DefaultDuration = CCTimeSpan.FromHours(1);
        public CCTask(int Id, string Title, string Description, int Priority, DateTime Deadline)
            : this(Id, Title, Description, Priority, Deadline, DefaultDuration){}
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public int Priority { get; } = Priority;
        
        public CCTimeSpan Duration { get; } = Duration;
        
        public DateTime Deadline { get; } = Deadline;
        
        public bool IsCompleted { get; set; } = false;
        
        public TaskPeriodic? Periodic { get; } = Periodic;

        public CCTask? CompleteTask()
        {
            IsCompleted = true;
            if (Periodic is null)
            {
                // If the task is not periodic, return null as it cannot be completed again
                return null;
            }

            CCTimeSpan newTimeSpan;
            // If the task is periodic, return a new instance with updated deadline
            switch (Periodic.PeriodicType)
            {
                case TaskPeriodicType.EveryWeekday:
                    newTimeSpan = CCTimeSpan.FromDays(Periodic.Span);
                    break;
                case TaskPeriodicType.EveryWeek:
                    newTimeSpan = CCTimeSpan.FromDays(Periodic.Span * 7);
                    break;
                case TaskPeriodicType.EveryMonth:
                    newTimeSpan = CCTimeSpan.FromDays(Periodic.Span * 30);
                    break;
                case TaskPeriodicType.EveryYear:
                    newTimeSpan = CCTimeSpan.FromDays(Periodic.Span * 365); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newDeadline = Deadline.Add(newTimeSpan.ToTimeSpan());
            return new CCTask(Id, Title, Description, Priority, newDeadline, Duration,
                Periodic);
        }
    }
}
