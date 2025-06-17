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
            return response.error ? null : response.result;
        }

        private ResponseHandler<HolidayList> LoadHolidays(DateTime startDate, DateTime endDate)
        {
            return new ResponseHandler<HolidayList>(
                _api.Get($"holidays?from={startDate:yyyy-MM-dd}&to={endDate:yyyy-MM-dd}"));
        }
        
        public void Dispose()
        {
        }
    }
}