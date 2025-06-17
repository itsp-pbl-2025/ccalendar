using System;
using LiteDB;

namespace Infrastructure.Data.Schema
{
    public class DHistoryContainer
    {
        [BsonId]
        public int Id { get; set; }
        
        public string Data { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}