using LiteDB;

namespace Infrastructure.Data.Schema
{
    public class DSchedule
    {
        [BsonId]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DScheduleDuration Duration { get; set; }
        public DSchedulePeriodic? Periodic { get; set; }
    }
}