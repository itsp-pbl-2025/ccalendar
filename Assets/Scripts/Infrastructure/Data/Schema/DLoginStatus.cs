using System;
using LiteDB;

namespace Infrastructure.Data.Schema
{
    public class DLoginStatus
    {
        [BsonId]
        public int Id { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public DLoginStatus(int id)
        {
            Id = id;
            LoginTime = DateTime.Now;
            LogoutTime = null;
        }
    }
}