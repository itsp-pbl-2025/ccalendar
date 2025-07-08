using System;
using Domain.Entity;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestCCDateTime
    {
        [Test]
        public void TestConversionCCDateTimeAndDateTime()
        {
            var dateTime = new DateTime(2025, 6, 24, 15, 30, 0, DateTimeKind.Local);

            var ccDateTime = new CCDateTime(dateTime);

            Assert.AreEqual(ccDateTime.ToDateTime(), dateTime);
        }

        [Test]
        public void TestCompareTo()
        {
            var ccDateTime1 = new CCDateTime(2025, 6, 24, 15, 30, 0);
            var ccDateTime2 = new CCDateTime(2025, 6, 24, 15, 31, 0);

            var result = ccDateTime1.CompareTo(ccDateTime2);

            Assert.Less(result, 0);
        }

        [Test]
        public void TestOperatorGreaterThan()
        {
            var earlier = new CCDateTime(new DateTime(2025, 6, 24, 15, 30, 0, DateTimeKind.Local));
            var later = new CCDateTime(new DateTime(2025, 6, 24, 15, 31, 0, DateTimeKind.Local));

            Assert.That(later > earlier, Is.True);
        }

        [Test]
        public void TestOperatorLessThan()
        {
            var earlier = new CCDateTime(2025, 6, 24, 15, 30, 0);
            var later = new CCDateTime(2025, 6, 24, 15, 31, 0);

            Assert.That(earlier < later, Is.True);
        }

        [Test]
        public void TestAddTime()
        {
            Assert.AreEqual(new DateTime(2027, 11, 30, 23, 59, 59, DateTimeKind.Local), new CCDateTime(2025, 11, 30, 23, 59, 59).AddYears(2).ToDateTime());
            Assert.AreEqual(new DateTime(2026, 1, 30, 23, 59, 59, DateTimeKind.Local), new CCDateTime(2025, 11, 30, 23, 59, 59).AddMonths(2).ToDateTime());
            Assert.AreEqual(new DateTime(2025, 12, 2, 23, 59, 59, DateTimeKind.Local), new CCDateTime(2025, 11, 30, 23, 59, 59).AddDays(2).ToDateTime());
            Assert.AreEqual(new DateTime(2025, 12, 1, 1, 59, 59, DateTimeKind.Local), new CCDateTime(2025, 11, 30, 23, 59, 59).AddHours(2).ToDateTime());
            Assert.AreEqual(new DateTime(2025, 12, 1, 0, 1, 59, DateTimeKind.Local), new CCDateTime(2025, 11, 30, 23, 59, 59).AddMinutes(2).ToDateTime());
            Assert.AreEqual(new DateTime(2025, 12, 1, 0, 0, 1, DateTimeKind.Local), new CCDateTime(2025, 11, 30, 23, 59, 59).AddSeconds(2).ToDateTime());
            
            Assert.AreEqual(new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Local), new CCDateTime(2024, 1, 31).AddMonths(1).ToDateTime());
        }
    }
}