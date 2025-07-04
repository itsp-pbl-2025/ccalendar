using System;
using System.Collections.Generic;
using AppCore.Interfaces;
using AppCore.Utilities;
using Cysharp.Threading.Tasks;
using Domain.Api;
using Domain.Entity;
using Domain.Enum;
using R3;

namespace AppCore.UseCases
{
    public class HolidayService : IService
    {
        private const int DefaultYearSpan = 5; //期間指定しないとき、現在時刻から前後ｎ年分を取得
        public const string HolidayNameFromApi = "休日";
        public const string SubstituteHolidayName = "振替休日";

        private readonly IContext _context;
        private readonly RequestHandler _api;

        private readonly Dictionary<CCDateOnly, string> _holidayMap = new();
        private readonly IScheduleRepository _scheduleRepo;
        
        private CCDateOnly _loadedSinceInclusive, _loadedUntilExclusive;
        
        public ReactiveProperty<bool> InitialLoaded { get; } = new(false); 

        public HolidayService(IContext context, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _loadedSinceInclusive = _loadedUntilExclusive = CCDateOnly.Today;
            
            _context = context;
            _api = new RequestHandler("https://holidays-jp.shogo82148.com/");
        }

        public string Name { get; }
        
        public void Setup()
        {
            LoadFromHistory();
            LoadHolidays(_loadedSinceInclusive.AddYears(-DefaultYearSpan), _loadedUntilExclusive.AddYears(DefaultYearSpan),
                _ => { InitialLoaded.Value = true;});
        }

        public void Dispose()
        {
        }

        public void LoadHolidays(CCDateOnly startDate, CCDateOnly endDate, Action<bool> loadingCallback = null)
        {
            // すでに読み込み範囲内だったら即返す
            if (_loadedSinceInclusive.CompareTo(startDate) <= 0 &&
                _loadedUntilExclusive.CompareTo(endDate) > 0)
            {
                loadingCallback?.Invoke(true);
                return;
            }
            
            LoadHolidaysAsync(startDate, endDate, loadingCallback).Forget();
        }

        public async UniTask<bool> LoadHolidaysAsync(CCDateOnly startDate, CCDateOnly endDate, Action<bool> loadingCallback = null)
        {
            const int maxRetries = 3; // 最大試行回数 (初回の1回 + リトライ2回)
            const int initialDelayMilliseconds = 1000; // 最初の待機時間 (1秒)

            // 順序が誤っていた場合、入れ替える
            if (startDate.CompareTo(endDate) > 0)
            {
                (startDate, endDate) = (endDate, startDate);
            }

            // 初期状態でない場合にキャッシュから離れた場所を読み込もうとしていたら、キャッシュと隣接するように範囲を書き換える
            if (_loadedSinceInclusive.CompareTo(_loadedUntilExclusive) != 0)
            {
                if (_loadedSinceInclusive.CompareTo(endDate) > 0)
                {
                    endDate = _loadedSinceInclusive.AddDays(-1);
                }

                if (_loadedUntilExclusive.CompareTo(startDate) <= 0)
                {
                    startDate = _loadedUntilExclusive;
                }
            }

            for (var i = 0; i < maxRetries; i++)
            {
                var response = await ApiGetHolidays(startDate, endDate);

                // --- 成功した場合 ---
                if (response.IsSuccess)
                {
                    CacheHolidayData(response.Data, startDate, endDate);
                    loadingCallback?.Invoke(true);
                    return true; // 成功したら終了
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
            loadingCallback?.Invoke(false);
            return false;
        }

        private async UniTask<RequestHandler.Result<HolidayList>> ApiGetHolidays(CCDateOnly startDate, CCDateOnly endDate)
        {
            var start = $"{startDate.Year.Value:D4}-{startDate.Month.Value:D2}-{startDate.Day.Value:D2}";
            var end = $"{endDate.Year.Value:D4}-{endDate.Month.Value:D2}-{endDate.Day.Value:D2}";

            return await _api.GetAsync<HolidayList>($"holidays?from={start}&to={end}");
        }

        private void CacheHolidayData(HolidayList list, CCDateOnly startDate, CCDateOnly endDate)
        {
            if (_loadedSinceInclusive.CompareTo(startDate) < 0 &&
                _loadedSinceInclusive.CompareTo(endDate.AddDays(1)) <= 0) _loadedSinceInclusive = startDate;
            if (_loadedUntilExclusive.CompareTo(endDate) <= 0 &&
                _loadedUntilExclusive.AddDays(1).CompareTo(startDate) <= 0) _loadedUntilExclusive = endDate.AddDays(1);
            
            var datesToRemove = new List<CCDateOnly>();
            foreach (var date in _holidayMap.Keys)
            {
                if (startDate.CompareTo(date) <= 0 && date.CompareTo(endDate) <= 0)
                    datesToRemove.Add(date);
            }

            foreach (var date in datesToRemove) _holidayMap.Remove(date);

            foreach (var h in list.holidays)
            {
                if (h.TryParseDate(out var dateOnly))
                {
                    _holidayMap[dateOnly] = h.name;
                }
            }
            
            SaveToHistory();
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

        private void LoadFromHistory()
        {
            var historyService = _context.GetService<HistoryService>();

            if (!historyService.TryGetHistory(HistoryType.CachedHolidays,
                    out Dictionary<CCDateOnly, string> cachedHolidays)) return;
            
            foreach (var (date, holidayName) in cachedHolidays)
            {
                _holidayMap.Add(date, holidayName);
            }
        }
        
        private void SaveToHistory()
        {
            _context.GetService<HistoryService>().UpdateHistory(HistoryType.CachedHolidays, _holidayMap);
        }
    }
}