using System;
using AppCore.Utilities;
using Domain.Entity;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestDateTimeExtensions
    {
        [Test]
        public void TestGetDayOfWeekWithIndex()
        {
            {
                var (index, dayOfWeek, isFinalWeek) = new CCDateOnly(2025, 7, 15).GetDayOfWeekWithIndex();

                Assert.AreEqual(3, index);
                Assert.AreEqual(DayOfWeek.Tuesday, dayOfWeek);
                Assert.IsFalse(isFinalWeek);
            }
            
            {
                var (index, dayOfWeek, isFinalWeek) = new CCDateOnly(2024, 10, 1).GetDayOfWeekWithIndex();

                Assert.AreEqual(1, index);
                Assert.AreEqual(DayOfWeek.Tuesday, dayOfWeek);
                Assert.IsFalse(isFinalWeek);
            }
            
            {
                var (index, dayOfWeek, isFinalWeek) = new CCDateOnly(2023, 12, 31).GetDayOfWeekWithIndex();

                Assert.AreEqual(5, index);
                Assert.AreEqual(DayOfWeek.Sunday, dayOfWeek);
                Assert.IsTrue(isFinalWeek);
            }
            
            {
                var (index, dayOfWeek, isFinalWeek) = new CCDateOnly(2022, 2, 22).GetDayOfWeekWithIndex();

                Assert.AreEqual(4, index);
                Assert.AreEqual(DayOfWeek.Tuesday, dayOfWeek);
                Assert.IsTrue(isFinalWeek);
            }
            
            {
                var (index, dayOfWeek, isFinalWeek) = new CCDateOnly(2021, 6, 29).GetDayOfWeekWithIndex();

                Assert.AreEqual(5, index);
                Assert.AreEqual(DayOfWeek.Tuesday, dayOfWeek);
                Assert.IsTrue(isFinalWeek);
            }
            
            {
                var (index, dayOfWeek, isFinalWeek) = new CCDateOnly(2020, 2, 29).GetDayOfWeekWithIndex();

                Assert.AreEqual(5, index);
                Assert.AreEqual(DayOfWeek.Saturday, dayOfWeek);
                Assert.IsTrue(isFinalWeek);
            }
        }

        [Test]
        public void TestGetIndexedWeekDay()
        {
            {
                var dateOnly = DateTimeExtensions.GetIndexedWeekDay(2025, 7, 3, DayOfWeek.Tuesday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2025, 7, 15)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetIndexedWeekDay(2024, 10, 1, DayOfWeek.Tuesday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2024, 10, 1)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetIndexedWeekDay(2023, 12, 5, DayOfWeek.Sunday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2023, 12, 31)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetIndexedWeekDay(2022, 2, 4, DayOfWeek.Tuesday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2022, 2, 22)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetIndexedWeekDay(2021, 6, 5, DayOfWeek.Tuesday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2021, 6, 29)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetIndexedWeekDay(2020, 2, 5, DayOfWeek.Saturday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2020, 2, 29)), 0);
            }
        }

        [Test]
        public void TestGetFinalWeekDay()
        {
            {
                var dateOnly = DateTimeExtensions.GetFinalWeekDay(2025, 7, DayOfWeek.Tuesday);
                Assert.AreNotEqual(dateOnly.CompareTo(new CCDateOnly(2025, 7, 15)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetFinalWeekDay(2024, 10, DayOfWeek.Tuesday);
                Assert.AreNotEqual(dateOnly.CompareTo(new CCDateOnly(2024, 10, 1)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetFinalWeekDay(2023, 12, DayOfWeek.Sunday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2023, 12, 31)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetFinalWeekDay(2022, 2, DayOfWeek.Tuesday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2022, 2, 22)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetFinalWeekDay(2021, 6, DayOfWeek.Tuesday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2021, 6, 29)), 0);
            }
            {
                var dateOnly = DateTimeExtensions.GetFinalWeekDay(2020, 2, DayOfWeek.Saturday);
                Assert.AreEqual(dateOnly.CompareTo(new CCDateOnly(2020, 2, 29)), 0);
            }
        }
    }
}