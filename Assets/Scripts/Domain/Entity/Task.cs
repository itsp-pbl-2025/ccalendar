using System;

namespace Domain.Entity
{
    public record Task(int Id, string Title, string Description, int Priority, DateTime Deadline)
    {
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public int Priority { get; } = Priority;
        public DateTime Deadline { get; } = Deadline;
    }
}
