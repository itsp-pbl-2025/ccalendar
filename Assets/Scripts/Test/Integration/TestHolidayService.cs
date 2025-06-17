using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.UseCases;
using Domain.Api;
using NUnit.Framework;
using Test.MockData;

namespace Test.Integration
{
    public class TestHolidayService
    {
        [Test]
        public async Task GetHolidays()
        {
            var ctx = InTestContext.Context;

            var service = ctx.GetService<HolidayService>();
            var (startDate, endDate) = MockHoliday.GetMockHolidayDuration();
            var holidays = await service.GetHolidays(startDate, endDate);
            
            Assert.IsNotNull(holidays);
            Assert.IsNotNull(holidays.holidays);
            
            var needFind = new HashSet<HolidayRaw>(holidays.holidays);
            foreach (var holiday in MockHoliday.GetMockHolidayRaws())
            {
                Assert.IsTrue(needFind.Remove(holiday));
            }
            Assert.IsEmpty(needFind);

            ctx.Dispose();
        }
        [Test]
        public async Task IsHoliday_And_GetHolidayName()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HolidayService>();
            var (startDate, endDate) = MockHoliday.GetMockHolidayDuration();
            await service.GetHolidays(startDate, endDate);

            foreach (var holiday in MockHoliday.GetMockHolidayRaws())
            {
                DateTime date = DateTime.Parse(holiday.date);
                Assert.IsTrue(service.IsHoliday(date));
                Assert.AreEqual(
                    holiday.name == "休日" ? "振替休日" : holiday.name,
                    service.GetHolidayName(date)
                );
            }

            DateTime nonHoliday = new DateTime(2022, 2, 1);
            Assert.IsFalse(service.IsHoliday(nonHoliday));
            Assert.IsNull(service.GetHolidayName(nonHoliday));

            ctx.Dispose();
        }
    }
}