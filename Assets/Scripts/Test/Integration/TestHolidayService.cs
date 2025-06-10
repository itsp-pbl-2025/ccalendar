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
        }
    }
}