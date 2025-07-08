using System;

namespace Domain.Entity
{
    public record CCTask(int Id, string Title, string Description, int Priority, CCDateTime Deadline)
    {
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public int Priority { get; } = Priority;
        public CCDateTime Deadline { get; } = Deadline;
    }
}
