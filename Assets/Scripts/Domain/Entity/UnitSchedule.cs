namespace Domain.Entity
{
    public record UnitSchedule(int Id, string Title, string Description, ScheduleDuration Duration)
    {
        public int Id { get; } = Id;
        public string Title { get; } = Title;
        public string Description { get; } = Description;
        public ScheduleDuration Duration { get; } = Duration;
    }
}