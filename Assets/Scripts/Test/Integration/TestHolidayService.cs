using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.UseCases;
using AppCore.Interfaces;
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
            //var ctx = InTestContext.Context;
            var startDate = new CCDateOnly(2022, 1, 1);
            var endDate = new CCDateOnly(2022, 12, 31);
            
            var repo = new FakeScheduleRepository();
            var service = new HolidayService(repo, startDate, endDate);

            if (!service.HolidayLoaded.Value)
                await service.HolidayLoaded.Where(x => x).FirstAsync();

            foreach (var raw in MockHoliday.GetMockHolidayRaws())
            {
                var d = new CCDateTime(DateTime.Parse(raw.date)).ToDateOnly();
                Assert.IsTrue(service.IsHoliday(d));
                Assert.IsTrue(service.TryGetHolidayName(d, out var name));
                Assert.AreEqual(
                    raw.name == HolidayService.HolidayNameFromApi ? HolidayService.SubstituteHolidayName : raw.name,
                    name);
            }

            var notd = new CCDateOnly(2022, 2, 1);
            Assert.IsFalse(service.IsHoliday(notd));
            Assert.IsFalse(service.TryGetHolidayName(notd, out var n));
            Assert.AreEqual(n, "");
            //ctx.Dispose();
        }
    }
    public class FakeScheduleRepository : IScheduleRepository
    {
        // 空実装
        public Schedule Insert(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public bool Update(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public bool InsertUpdate(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public ICollection<Schedule> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}