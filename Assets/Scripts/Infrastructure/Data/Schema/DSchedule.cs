using Domain.Enum;
using LiteDB;

namespace Infrastructure.Data.Schema
{
    public class DSchedule
    {
        [BsonId]
        public int Id { get; set; }
        
        public ScheduleType Type { get; set; }
        public string Title { get; set; }
    }
}