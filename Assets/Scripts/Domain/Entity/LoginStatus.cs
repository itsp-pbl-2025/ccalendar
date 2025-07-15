//LoginStatus.cs

using System;

namespace Domain.Entity
{
    public class LoginStatus
    {
        public int Id { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
}
