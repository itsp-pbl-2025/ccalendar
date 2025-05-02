using System;
using LiteDB;

namespace Database.Model
{
    public class DLoginStatus
    {
        [BsonId]
        public int Id { get; set; }
        
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}