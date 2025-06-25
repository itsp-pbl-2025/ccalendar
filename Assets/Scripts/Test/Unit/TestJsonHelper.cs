using System;
using System.Collections.Generic;
using AppCore.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestJsonHelper
    {
        [Test]
        public void TestToJson()
        {
            // float (primitive test)
            Assert.AreEqual(JsonConvert.SerializeObject(123.45f), "123.45");
            
            // List<int> (List test)
            Assert.AreEqual(JsonConvert.SerializeObject(new List<int> {1, 2, 3, 4}), "[1,2,3,4]");
            
            // Dictionary<int, string>
            Assert.AreEqual(
                JsonConvert.SerializeObject(new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" }, { 4, "four" }}),
                "{\"1\":\"one\",\"2\":\"two\",\"3\":\"three\",\"4\":\"four\"}");
            
            // DateTime (sample test 1)
            var time = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(JsonConvert.SerializeObject(time.ToTimeZoneTokyo()), "\"2024-10-01T09:00:00\"");
        }

        [Test]
        public void TestFromJson()
        {
            // float (primitive test)
            Assert.AreEqual(123.45f, JsonConvert.DeserializeObject<float>("123.45"));
            
            // List<int> (List test)
            Assert.AreEqual(new List<int> {1, 2, 3, 4}, JsonConvert.DeserializeObject<List<int>>("[1,2,3,4]"));
            
            // Dictionary<int, string>
            Assert.AreEqual(
                new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" }, { 4, "four" }},
                JsonConvert.DeserializeObject<Dictionary<int, string>>("{\"1\":\"one\",\"2\":\"two\",\"3\":\"three\",\"4\":\"four\"}"));
            
            // DateTime (sample test 1)
            var time = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(time.ToTimeZoneTokyo(), JsonConvert.DeserializeObject<DateTime>("\"2024-10-01T09:00:00\""));
        }
    }
}