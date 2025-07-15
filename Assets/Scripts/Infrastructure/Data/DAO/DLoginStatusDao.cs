using Domain.Entity;
using Infrastructure.Data.Schema;

namespace Infrastructure.Data.DAO
{
    public static class DLoginStatusDao
    {
        public static DLoginStatus FromDomain(this LoginStatus loginStatus)
        {
            return new DLoginStatus(loginStatus.Id)
            {
                LogoutTime = loginStatus.LogoutTime,
            };
        }

        public static LoginStatus ToDomain(this DLoginStatus dLoginStatus)
        {
            return new LoginStatus
            {
                Id = dLoginStatus.Id,
                LoginTime = dLoginStatus.LoginTime,
                LogoutTime = dLoginStatus.LogoutTime
            };
        }
    }
}