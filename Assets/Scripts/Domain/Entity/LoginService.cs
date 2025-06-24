using LiteDB;

namespace Domain.Entity;
{
    public class LoginService : IDisposable
    {
        private static List<DLoginStatus> _loginHistory = new List<DLoginStatus>();
        private DLoginStatus _currentSession;

        public LoginService(int userId)
        {
            _currentSession = new DLoginStatus(userId);
            _loginHistory.Add(_currentSession);
        }

        public DateTime GetCurrentLoginTime()
        {
            return _currentSession.LoginTime;
        }

        public DateTime? GetPreviousLoginTime()
        {
            return _loginHistory
                .Where(l => l.Id == _currentSession.Id && l != _currentSession)
                .OrderByDescending(l => l.LoginTime)
                .FirstOrDefault()?.LoginTime;
        }
    
        public List<DLoginStatus> GetAllLoginStatus()
        {
            return _loginHistory
                .Where(l => l.Id == _currentSession.Id)
                .ToList();
        }

        public void Dispose()
        {
            _currentSession.LogoutTime = DateTime.Now;
        }
    }
}