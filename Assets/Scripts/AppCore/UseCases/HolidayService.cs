using System;
using AppCore.Interfaces;
using AppCore.Utilities;
using Cysharp.Threading.Tasks;
using Domain.Api;

namespace AppCore.UseCases
{
    public class HolidayService : IService
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly RequestHandler _api;
        
        public string Name { get; }
        
        public void Setup() {}
        
        public HolidayService(IScheduleRepository repo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _scheduleRepo = repo;

            _api = new RequestHandler("https://holidays-jp.shogo82148.com/");
        }

        public async UniTask<HolidayList> GetHolidays(DateTime startDate, DateTime endDate)
        {
            return await LoadHolidays(startDate, endDate);
        }

        private async UniTask<HolidayList> LoadHolidays(DateTime startDate, DateTime endDate)
        {
            var result = await _api.GetAsync<HolidayList>($"holidays?from={startDate:yyyy-MM-dd}&to={endDate:yyyy-MM-dd}");
            return result.Data;
        }
        
        public void Dispose()
        {
        }
    }
}