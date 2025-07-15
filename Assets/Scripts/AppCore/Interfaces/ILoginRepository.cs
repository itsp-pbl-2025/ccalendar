using System.Collections.Generic;
using Domain.Entity;

namespace AppCore.Interfaces
{
    public interface ILoginRepository
    {
        public LoginStatus Insert(LoginStatus loginStatus);
        public bool Update(LoginStatus loginStatus);
        public bool InsertUpdate(LoginStatus loginStatus);
        public bool Remove(LoginStatus loginStatus);
        public ICollection<LoginStatus> GetAll();
    }
}