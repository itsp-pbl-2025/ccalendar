using Domain.Entity;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestDateRange
    {
        [Test]
        public void TestIsBetween()
        {
            var startDate = new CCDateTime(2025, 5, 20, 15, 2, 30);
            var endDate = new CCDateTime(2025, 5, 20, 16, 2, 30);
            var range = endDate - startDate;

            Assert.That(startDate.IsBetween(startDate, endDate), Is.True, "Start date should be in range");
            Assert.That(endDate.IsBetween(startDate,endDate), Is.True, "End date should be in range");
            Assert.That(new CCDateTime(2025, 5, 20, 15, 30, 30).IsBetween(startDate, endDate), Is.True, "Middle date should be in range");
            Assert.That(new CCDateTime(2025, 5, 20, 14, 2, 30).IsBetween(startDate, endDate), Is.False, "Date before range should not be in range");
            Assert.That(new CCDateTime(2025, 5, 20, 17, 2, 30).IsBetween(startDate, endDate), Is.False, "Date after range should not be in range");
        }
    }
}
