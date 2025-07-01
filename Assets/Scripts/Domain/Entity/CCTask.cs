using System;

namespace Domain.Entity
{
    public record CCTask(int Id, string Title, string Description, int Priority, DateTime Deadline, TimeSpan Duration)
    {
        private const TimeSpan DefaultDuration = TimeSpan.FromHours(1);
        public CCTask(int Id, string Title, string Description, int Priority, DateTime Deadline)
            : this(Id, Title, Description, Priority, Deadline, DefaultDuration){}
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public int Priority { get; } = Priority;
        public TimeSpan Duration { get; } = Duration; 
        public DateTime Deadline { get; } = Deadline;
    }
}
