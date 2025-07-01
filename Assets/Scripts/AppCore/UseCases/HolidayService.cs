using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using AppCore.Utilities;
using Cysharp.Threading.Tasks;
using Domain.Api;
using Domain.Entity;
using R3;

namespace AppCore.UseCases
{
    public class HolidayService : IService
    {
        private const int DefaultYearSpan = 5; //期間指定しないとき、現在時刻から前後ｎ年分を取得
        public const string HolidayNameFromApi = "休日";
        public const string SubstituteHolidayName = "振替休日";

        private readonly RequestHandler _api;

        private readonly Dictionary<CCDateOnly, string> _holidayMap = new();
        private readonly IScheduleRepository _scheduleRepo;


        public HolidayService(
            CCDateOnly startDate,
            CCDateOnly endDate,
            string name = "")
        {
            Name = name != "" ? name : GetType().Name;

            _api = new RequestHandler("https://holidays-jp.shogo82148.com/");
            GetHolidays(startDate, endDate).Forget();
        }

        public HolidayService(string name = "")
            : this(
                new CCDateOnly(DateTime.Today.Year - DefaultYearSpan, 1, 1), // 5 年前 1/1
                new CCDateOnly(DateTime.Today.Year + DefaultYearSpan, 12, 31), // 5 年後 12/31
                name)
        {
        }

        public ReactiveProperty<bool> HolidayLoaded { get; } = new(false);
        public string Name { get; }


        public void Dispose()
        {
            HolidayLoaded?.Dispose();
        }


        private async UniTask GetHolidays(CCDateOnly startDate, CCDateOnly endDate)
        {
            const int maxRetries = 3; // 最大試行回数 (初回の1回 + リトライ2回)
            const int initialDelayMilliseconds = 1000; // 最初の待機時間 (1秒)

            for (var i = 0; i < maxRetries; i++)
            {
                var response = await LoadHolidays(startDate, endDate);

                // --- 成功した場合 ---
                if (response.IsSuccess)
                {
                    CacheHolidayData(response.Data, startDate, endDate);
                    HolidayLoaded.Value = true;
                    return; // 成功したら終了
                }

                // --- 失敗した場合 ---
                // これが最後の試行だった場合は、ループを抜けて最終処理へ
                if (i >= maxRetries - 1) break;

                // 待機時間をリトライごとに長くする
                var delay = initialDelayMilliseconds * (1 << i);

                // UniTask.Delayを使用して、メインスレッドをブロックせずに待機する
                await UniTask.Delay(delay);
            }

            // --- 全てのリトライが失敗した場合
            var emptyData = new HolidayList
            {
                holidays = new List<HolidayRaw>()
            };
            CacheHolidayData(emptyData, startDate, endDate);
            HolidayLoaded.Value = true;
        }

        private async UniTask<RequestHandler.Result<HolidayList>> LoadHolidays(CCDateOnly startDate, CCDateOnly endDate)
        {
            var start = $"{startDate.Year.Value:D4}-{startDate.Month.Value:D2}-{startDate.Day.Value:D2}";
            var end = $"{endDate.Year.Value:D4}-{endDate.Month.Value:D2}-{endDate.Day.Value:D2}";

            return await _api.GetAsync<HolidayList>($"holidays?from={start}&to={end}");
        }

        private void CacheHolidayData(HolidayList list, CCDateOnly startDate, CCDateOnly endDate)
        {
            var datesToRemove = new List<CCDateOnly>();
            foreach (var date in _holidayMap.Keys)
                if (startDate.CompareTo(date) <= 0 && date.CompareTo(endDate) <= 0)
                    datesToRemove.Add(date);

            foreach (var date in datesToRemove) _holidayMap.Remove(date);

            foreach (var h in list.holidays)
                if (DateTime.TryParse(h.date, out var parsedDate))
                {
                    var key = new CCDateOnly(parsedDate.Year, parsedDate.Month, parsedDate.Day);
                    _holidayMap[key] = h.name;
                }
        }

        public bool IsHoliday(CCDateOnly date)
        {
            return _holidayMap.ContainsKey(date);
        }

        public bool TryGetHolidayName(CCDateOnly date, out string name)
        {
            if (_holidayMap.TryGetValue(date, out name))
            {
                if (name == HolidayNameFromApi) name = SubstituteHolidayName;
                return true;
            }

            name = "";
            return false;
        }
    }
}