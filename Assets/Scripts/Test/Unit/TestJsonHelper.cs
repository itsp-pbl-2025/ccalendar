using System;
using System.Collections.Generic;
using AppCore.Utilities;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestJsonHelper
    {
        [Test]
        public void TestToJson()
        {
            // float (primitive test)
            Assert.AreEqual(JsonHelper.ToJson(123.45f), "123.45");
            
            // List<int> (List test)
            Assert.AreEqual(JsonHelper.ToJson(new List<int> {1, 2, 3, 4}), "[1,2,3,4]");
            
            // Dictionary<int, string>
            Assert.AreEqual(
                JsonHelper.ToJson(new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" }, { 4, "four" }}),
                "[{\"Key\":1,\"Value\":\"one\"},{\"Key\":2,\"Value\":\"two\"},{\"Key\":3,\"Value\":\"three\"},{\"Key\":4,\"Value\":\"four\"}]");
            
            // DateTime (sample test 1)
            var utcDateTime = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var tokyoTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tokyoTimeZone);
            Assert.AreEqual(
                JsonHelper.ToJson(tokyoTime),
                "\"\\/Date(1727740800000+0900)\\/\"");
        }

        [Test]
        public void TestFromJson()
        {
            // float (primitive test)
            Assert.AreEqual(123.45f, JsonHelper.FromJson<float>("123.45"));
            
            // List<int> (List test)
            Assert.AreEqual(new List<int> {1, 2, 3, 4}, JsonHelper.FromJson<List<int>>("[1,2,3,4]"));
            
            // Dictionary<int, string>
            Assert.AreEqual(
                new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" }, { 4, "four" }},
                JsonHelper.FromJson<Dictionary<int, string>>("[{\"Key\":1,\"Value\":\"one\"},{\"Key\":2,\"Value\":\"two\"},{\"Key\":3,\"Value\":\"three\"},{\"Key\":4,\"Value\":\"four\"}]"));
            
            // DateTime (sample test 1)
            var utcDateTime = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var tokyoTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tokyoTimeZone);
            Assert.AreEqual(tokyoTime, JsonHelper.FromJson<DateTime>("\"\\/Date(1727740800000+0900)\\/\""));
        }
    }
}