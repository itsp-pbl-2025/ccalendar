using System;

namespace Domain.Entity
{
    public record CCTask(int Id, string Title, string Description, int Priority, DateTime Deadline, CCTimeSpan Duration)
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
    }
}
