using LiteDB;
using System.Collections.Generic;
using Domain.Entity;
using AppCore.Interfaces;
using Infrastructure.Data.Schema;
using Infrastructure.Data.DAO;

namespace Infrastructure.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly ILiteCollection<DLoginStatus> _col;

        public LoginRepository(LiteDatabase db) => _col = db.GetCollection<DLoginStatus>("loginStatus");

        public LoginStatus Insert(LoginStatus loginStatus)
        {
            var dLoginStatus = loginStatus.FromDomain();
            dLoginStatus.Id = _col.Insert(dLoginStatus).AsInt32;
            return dLoginStatus.ToDomain();
        }
        
        public bool Update(LoginStatus loginStatus)
        {
            var dLoginStatus = loginStatus.FromDomain();
            return _col.Update(dLoginStatus);
        }
        
        public bool InsertUpdate(LoginStatus loginStatus)
        {
            var dLoginStatus = loginStatus.FromDomain();
            return _col.Upsert(dLoginStatus);
        }
        
        public bool Remove(LoginStatus loginStatus)
        {
            return _col.Delete(loginStatus.Id);
        }
        
        public ICollection<LoginStatus> GetAll()
        {
            var result = new List<LoginStatus>();
            foreach (var loginStatus in _col.FindAll())
            {
                result.Add(loginStatus.ToDomain());
            }
            return result;
        }
    }
}