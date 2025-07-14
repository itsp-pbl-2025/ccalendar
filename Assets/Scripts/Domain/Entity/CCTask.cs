using System;
using Domain.Enum;

namespace Domain.Entity
{
    public record CCTask(int Id, string Title, string Description, int Priority, CCDateTime Deadline, CCTimeSpan Duration, bool IsCompleted = false)
    {
        private static readonly CCTimeSpan DefaultDuration = CCTimeSpan.FromHours(1);
        
        public CCTask(int Id, string Title, string Description, int Priority, CCDateTime Deadline)
            : this(Id, Title, Description, Priority, Deadline, DefaultDuration){}
        
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public int Priority { get; } = Priority;
        public CCTimeSpan Duration { get; } = Duration;

        public bool IsCompleted { get; set; } = IsCompleted;
        public CCDateTime Deadline { get; } = Deadline;
    }
}
