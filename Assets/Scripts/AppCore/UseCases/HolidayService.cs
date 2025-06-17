using System;
using AppCore.Interfaces;
using Cysharp.Threading.Tasks;
using Domain.Api;
using WebRequest;

namespace AppCore.UseCases
{
    public class HolidayService : IService
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly RequestHandler _api;
        
        private Dictionary<string, string> _holidayMap = new();
        
        public string Name { get; }
        
        public HolidayService(IScheduleRepository repo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _scheduleRepo = repo;

            _api = new RequestHandler("https://holidays-jp.shogo82148.com/");
        }

        public async UniTask<HolidayList> GetHolidays(DateTime startDate, DateTime endDate)
        {
            var response = await LoadHolidays(startDate, endDate).ToUniTask();
            if (response.error) return null;

            CacheHolidayData(response.result);
            return response.result;
        }

        private ResponseHandler<HolidayList> LoadHolidays(DateTime startDate, DateTime endDate)
        {
            return new ResponseHandler<HolidayList>(
                _api.Get($"holidays?from={startDate:yyyy-MM-dd}&to={endDate:yyyy-MM-dd}"));
        }
        
        private void CacheHolidayData(HolidayList list)
        {
            _holidayMap.Clear();
            foreach (var h in list.holidays)
            {
                _holidayMap[h.date] = h.name;
            }
        }
        
        public bool IsHoliday(DateTime date)
        {
            string key = date.ToString("yyyy-MM-dd");
            return _holidayMap.ContainsKey(key);
        }
        
        public string GetHolidayName(DateTime date)
        {
            string key = date.ToString("yyyy-MM-dd");
    
            if (_holidayMap.TryGetValue(key, out string name))
            {
                // "休日" の場合は "振替休日" に変換して返す
                return name == "休日" ? "振替休日" : name;
            }

            return null;
        }
        
        public void Dispose()
        {
        }
    }
}