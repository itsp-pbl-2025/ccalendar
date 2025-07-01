using System;
using LiteDB;

namespace Infrastructure.Data.Schema
{
    public class DScheduleTask
    {
        [BsonId]
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime Deadline { get; set; }
    }
}