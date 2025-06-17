using Domain.Entity;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestDataRange
    {
        [Test]
        public void TestIsBetween()
        {
            var ctx = InTestContext.Context;

            var startDate = new CCDateTime(2025, 5, 20, 15, 2, 30);
            var endDate = new CCDateTime(2025, 5, 20, 16, 2, 30);
            var range = new CCTimeSpan(startDate, endDate);

            Assert.IsTrue(range.IsBetween(startDate));
            Assert.IsTrue(range.IsBetween(endDate));
            Assert.IsTrue(range.IsBetween(new CCDateTime(2025, 5, 20, 15, 30, 30)));
            Assert.IsFalse(range.IsBetween(new CCDateTime(2025, 5, 20, 14, 2, 30)));
            Assert.IsFalse(range.IsBetween(new CCDateTime(2025, 5, 20, 17, 2, 30)));
            
            ctx.Dispose();
        }
    }
}