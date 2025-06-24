using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using R3;
using AppCore.UseCases;
using Domain.Api;
using Domain.Entity;
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
            var (startDt, endDt) = MockHoliday.GetMockHolidayDuration();

            if (!service.HolidayLoaded.Value)
            	await service.HolidayLoaded.Where(x => x).FirstAsync();

            foreach (var raw in MockHoliday.GetMockHolidayRaws())
    		{
        		CCDateOnly d = new CCDateTime(DateTime.Parse(raw.date)).ToDateOnly();
        		Assert.IsTrue(service.IsHoliday(d));
        		Assert.IsTrue(service.GetHolidayName(d, out var name));
        		Assert.AreEqual(raw.name == "休日" ? "振替休日" : raw.name, name);
    		}

            ctx.Dispose();
        }

    }
}