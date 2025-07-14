using System;
using LiteDB;

namespace Infrastructure.Data.Schema
{
    public class DCCTask
    {
        [BsonId]
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime Deadline { get; set; }
        public TimeSpan Duration { get; set; }

        public bool IsCompleted { get; set; }
    }
}