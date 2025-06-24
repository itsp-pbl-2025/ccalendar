using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using Cysharp.Threading.Tasks;
using Domain.Api;
using Domain.Entity;
using WebRequest;
using R3; 

namespace AppCore.UseCases
{
    public class HolidayService : IService
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly RequestHandler _api;
        
        private Dictionary<CCDateOnly, string> _holidayMap = new();
        
        public string Name { get; }
        
        public ReactiveProperty<bool> HolidayLoaded { get; } = new(false);
        
        private const int DefaultYearSpan = 5; //期間指定しないとき、現在時刻から前後ｎ年分を取得
        
        
        public HolidayService(
            IScheduleRepository repo,
            CCDateOnly startDate,
            CCDateOnly endDate,
            string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _scheduleRepo = repo;

            _api = new RequestHandler("https://holidays-jp.shogo82148.com/");
            GetHolidays(startDate, endDate).Forget();
        }
		public HolidayService(IScheduleRepository repo, string name = "")
            : this(
                repo,
                new CCDateOnly(DateTime.Today.Year - DefaultYearSpan, 1, 1),   // 5 年前 1/1
                new CCDateOnly(DateTime.Today.Year + DefaultYearSpan, 12, 31), // 5 年後 12/31
                name)
        { }


        private async UniTask GetHolidays(CCDateOnly startDate, CCDateOnly endDate)
        {
            var response = await LoadHolidays(startDate, endDate).ToUniTask();
            if (response.error)
            {
                // 失敗時の処理
                return;
            }

            CacheHolidayData(response.result);
            
            // 読み込み完了を通知
            HolidayLoaded.Value = true;
        }

        private ResponseHandler<HolidayList> LoadHolidays(CCDateOnly startDate, CCDateOnly endDate)
        {
            string start = $"{startDate.Year.Value:D4}-{startDate.Month.Value:D2}-{startDate.Day.Value:D2}";
			string end = $"{endDate.Year.Value:D4}-{endDate.Month.Value:D2}-{endDate.Day.Value:D2}";
			return new ResponseHandler<HolidayList>(
                _api.Get($"holidays?from={start}&to={end}"));
        }
        
        private void CacheHolidayData(HolidayList list)
        {
            _holidayMap.Clear();
            foreach (var h in list.holidays)
            {
                if (DateTime.TryParse(h.date, out var parsedDate))
                {
                    var key = new CCDateOnly(parsedDate.Year, parsedDate.Month, parsedDate.Day);
                    _holidayMap[key] = h.name;
                }
            }
        }
        
        public bool IsHoliday(CCDateOnly date)
        {
            return _holidayMap.ContainsKey(date);
        }
        public bool GetHolidayName(CCDateOnly date, out string name)
        {
            if (_holidayMap.TryGetValue(date, out name))
            {
                if (name == "休日") name = "振替休日";
                return true;
            }

            name = "";
            return false;
        }


        public void Dispose()
        {
            HolidayLoaded?.Dispose();
        }
    }
}