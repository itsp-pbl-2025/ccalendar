using System;
using System.Collections.Generic;
using Domain.Api;
using Domain.Entity;

namespace Test.MockData
{
    public static class MockHoliday
    {
        public static (CCDateOnly start, CCDateOnly end) GetMockHolidayDuration()
        {
            return (new CCDateOnly(2022, 1, 1), new CCDateOnly(2022, 12, 31));
        }
        
        /// <returns>2022年の祝日一覧</returns>
        public static List<HolidayRaw> GetMockHolidayRaws()
        {
            return new List<HolidayRaw>
            {
                new HolidayRaw
                {
                    date = "2022-01-01",
                    name = "元日",
                },
                new HolidayRaw
                {
                    date = "2022-01-10",
                    name = "成人の日",
                },
                new HolidayRaw
                {
                    date = "2022-02-11",
                    name = "建国記念の日",
                },
                new HolidayRaw
                {
                    date = "2022-02-23",
                    name = "天皇誕生日",
                },
                new HolidayRaw
                {
                    date = "2022-03-21",
                    name = "春分の日",
                },
                new HolidayRaw
                {
                    date = "2022-04-29",
                    name = "昭和の日",
                },
                new HolidayRaw
                {
                    date = "2022-05-03",
                    name = "憲法記念日",
                },
                new HolidayRaw
                {
                    date = "2022-05-04",
                    name = "みどりの日",
                },
                new HolidayRaw
                {
                    date = "2022-05-05",
                    name = "こどもの日",
                },
                new HolidayRaw
                {
                    date = "2022-07-18",
                    name = "海の日",
                },
                new HolidayRaw
                {
                    date = "2022-08-11",
                    name = "山の日",
                },
                new HolidayRaw
                {
                    date = "2022-09-19",
                    name = "敬老の日",
                },
                new HolidayRaw
                {
                    date = "2022-09-23",
                    name = "秋分の日",
                },
                new HolidayRaw
                {
                    date = "2022-10-10",
                    name = "スポーツの日",
                },
                new HolidayRaw
                {
                    date = "2022-11-03",
                    name = "文化の日",
                },
                new HolidayRaw
                {
                    date = "2022-11-23",
                    name = "勤労感謝の日",
                },
            };
        }

        public static CCDateOnly GetRandomNormalDay()
        {
            var holidays = new HashSet<CCDateOnly>();
            foreach (var holidayRaw in GetMockHolidayRaws())
            {
                if (holidayRaw.TryParseDate(out var date))
                {
                    holidays.Add(date);
                }
            }
            
            var random = new Random();
            var dayInitial = new CCDateOnly(2022, 1, 1);
            while (true)
            {
                var date = dayInitial.AddDays(random.Next(0, 364));
                if (holidays.Contains(date)) continue;
                return date;
            }
        }
    }
}