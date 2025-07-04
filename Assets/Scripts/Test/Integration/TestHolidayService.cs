using System;
using System.Threading.Tasks;
using AppCore.UseCases;
using Domain.Entity;
using NUnit.Framework;
using R3;
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
            var success = await service.LoadHolidaysAsync(startDate, endDate);
            Assert.IsTrue(success);

            foreach (var raw in MockHoliday.GetMockHolidayRaws())
            {
                var d = new CCDateTime(DateTime.Parse(raw.date)).ToDateOnly();
                Assert.IsTrue(service.IsHoliday(d));
                Assert.IsTrue(service.TryGetHolidayName(d, out var name));
                Assert.AreEqual(
                    raw.name == HolidayService.HolidayNameFromApi ? HolidayService.SubstituteHolidayName : raw.name,
                    name);
            }

            var normalDay = MockHoliday.GetRandomNormalDay();
            Assert.IsFalse(service.IsHoliday(normalDay));
            Assert.IsFalse(service.TryGetHolidayName(normalDay, out var n));
            Assert.AreEqual(n, "");
            
            if (!service.InitialLoaded.Value) await service.InitialLoaded.Where(x => x).FirstAsync();
            ctx.Dispose();
        }
    }
}