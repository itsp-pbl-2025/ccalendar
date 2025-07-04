using System;
using AppCore.UseCases;
using AppCore.Utilities;
using Domain.Entity;
using Domain.Enum;
using NUnit.Framework;

namespace Test.Integration
{
    public class TestHistoryService
    {
        [Test]
        public void TestUpdateHistory()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();
            
            Assert.IsFalse(service.ContainsHistory((HistoryType)1));
            
            Assert.IsTrue(service.UpdateHistory((HistoryType)1, 123));
            Assert.IsTrue(service.ContainsHistory((HistoryType)1));
            Assert.IsFalse(service.UpdateHistory((HistoryType)1, 456));
            Assert.IsTrue(service.ContainsHistory((HistoryType)1));
            
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result));
            Assert.AreEqual(456, result);
            
            Assert.IsTrue(service.UpdateHistory((HistoryType)2, 137438691328UL));
            Assert.AreEqual(service.GetHistoryOrDefault<ulong>((HistoryType)2), 137438691328UL);
            
            Assert.IsTrue(service.UpdateHistory((HistoryType)3, 123.45f));
            Assert.AreEqual(service.GetHistoryOrDefault<float>((HistoryType)3), 123.45f);
            
            Assert.IsTrue(service.UpdateHistory((HistoryType)4, "institute of science tokyo"));
            Assert.AreEqual(service.GetHistoryOrDefault<string>((HistoryType)4), "institute of science tokyo");

            var ccDt = new CCDateTime(2024, 10, 1, 12, 0, 0);
            Assert.IsTrue(service.UpdateHistory((HistoryType)5, ccDt));
            Assert.AreEqual(service.GetHistoryOrDefault<CCDateTime>((HistoryType)5), ccDt);
            
            var ccDo = new CCDateOnly(2024, 10, 1);
            Assert.IsTrue(service.UpdateHistory((HistoryType)6, ccDo));
            Assert.AreEqual(service.GetHistoryOrDefault<CCDateOnly>((HistoryType)6), ccDo);
            
            var ccTo = new CCTimeOnly(12, 34, 56);
            Assert.IsTrue(service.UpdateHistory((HistoryType)7, ccTo));
            Assert.AreEqual(service.GetHistoryOrDefault<CCTimeOnly>((HistoryType)7), ccTo);
            
            var dt = new DateTime(2024, 10, 1, 12, 0, 0);
            Assert.IsTrue(service.UpdateHistory((HistoryType)8, dt));
            Assert.AreEqual(service.GetHistoryOrDefault<DateTime>((HistoryType)8), dt);
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestUpdateHistoryIfGreater()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();

            Assert.IsTrue(service.UpdateHistoryIfGreater((HistoryType)1, 456));
            Assert.IsFalse(service.UpdateHistoryIfGreater((HistoryType)1, 123));
            Assert.IsFalse(service.UpdateHistoryIfGreater((HistoryType)1, 456));
            Assert.IsTrue(service.ContainsHistory((HistoryType)1));
            
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result1));
            Assert.AreEqual(result1, 456);
            
            Assert.IsTrue(service.UpdateHistoryIfGreater((HistoryType)1, 789));
            Assert.IsFalse(service.UpdateHistoryIfGreater((HistoryType)1, 789));
            
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result2));
            Assert.AreEqual(result2, 789);
            
            Assert.IsTrue(service.UpdateHistoryIfGreater((HistoryType)1, 0, (_, _) => true));
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result3));
            Assert.AreEqual(result3, 0);
            
            Assert.IsFalse(service.UpdateHistoryIfGreater((HistoryType)1, 999, (_, _) => false));
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result4));
            Assert.AreEqual(result4, 0);
            
            Assert.IsTrue(service.UpdateHistoryIfGreater((HistoryType)2, new DateTime(2023, 4, 11)));
            Assert.IsTrue(service.UpdateHistoryIfGreater((HistoryType)2, new DateTime(2024, 10, 1)));
            Assert.IsFalse(service.UpdateHistoryIfGreater((HistoryType)2, new DateTime(2023, 12, 13)));
            Assert.IsTrue(service.TryGetHistory((HistoryType)2, out DateTime result5));
            Assert.AreEqual(result5, new DateTime(2024, 10, 1));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestUpdateHistoryIfLesser()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();

            Assert.IsTrue(service.UpdateHistoryIfLesser((HistoryType)1, 456));
            Assert.IsFalse(service.UpdateHistoryIfLesser((HistoryType)1, 789));
            Assert.IsFalse(service.UpdateHistoryIfLesser((HistoryType)1, 456));
            Assert.IsTrue(service.ContainsHistory((HistoryType)1));
            
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result1));
            Assert.AreEqual(result1, 456);
            
            Assert.IsTrue(service.UpdateHistoryIfLesser((HistoryType)1, 123));
            Assert.IsFalse(service.UpdateHistoryIfLesser((HistoryType)1, 123));
            
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result2));
            Assert.AreEqual(result2, 123);
            
            Assert.IsTrue(service.UpdateHistoryIfLesser((HistoryType)1, 999, (_, _) => true));
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result3));
            Assert.AreEqual(result3, 999);
            
            Assert.IsFalse(service.UpdateHistoryIfLesser((HistoryType)1, 0, (_, _) => false));
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result4));
            Assert.AreEqual(result4, 999);
            
            Assert.IsTrue(service.UpdateHistoryIfLesser((HistoryType)2, new DateTime(2023, 12, 13)));
            Assert.IsFalse(service.UpdateHistoryIfLesser((HistoryType)2, new DateTime(2024, 10, 1)));
            Assert.IsTrue(service.UpdateHistoryIfLesser((HistoryType)2, new DateTime(2023, 4, 11)));
            Assert.IsTrue(service.TryGetHistory((HistoryType)2, out DateTime result5));
            Assert.AreEqual(result5, new DateTime(2023, 4, 11));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestContainsHistory()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();
            
            Assert.IsFalse(service.ContainsHistory((HistoryType)1));
            
            service.UpdateHistory((HistoryType)1, 123);
            Assert.IsTrue(service.ContainsHistory((HistoryType)1));
            service.UpdateHistory((HistoryType)1, 456);
            Assert.IsTrue(service.ContainsHistory((HistoryType)1));
            
            service.RemoveHistory((HistoryType)1);
            Assert.IsFalse(service.ContainsHistory((HistoryType)1));
            Assert.IsFalse(service.ContainsHistory((HistoryType)2));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestTryGetHistory()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();
            
            Assert.IsFalse(service.TryGetHistory((HistoryType)1, out int _));
            
            service.UpdateHistory((HistoryType)1, 123);
            Assert.IsTrue(service.TryGetHistory((HistoryType)1, out int result1));
            Assert.AreEqual(result1, 123);
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestGetHistoryOrDefault()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();
            
            Assert.AreEqual(service.GetHistoryOrDefault<int>((HistoryType)1), 0);
            
            service.UpdateHistory((HistoryType)1, 123);
            Assert.AreEqual(service.GetHistoryOrDefault<int>((HistoryType)1), 123);
            
            Assert.AreEqual(service.GetHistoryOrDefault<DateTime>((HistoryType)2), new DateTime());
            service.UpdateHistory((HistoryType)2, new DateTime(2024, 10, 1));
            Assert.AreEqual(service.GetHistoryOrDefault<DateTime>((HistoryType)2), new DateTime(2024, 10, 1));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestRawHistory()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();
            
            Assert.IsNull(service.GetRawHistory((HistoryType)1));
            
            service.UpdateHistory((HistoryType)1, 123.45f);
            var raw1 =  service.GetRawHistory((HistoryType)1);
            Assert.IsNotNull(raw1);
            _ = Convert.FromBase64String(raw1.Data); // Base64エンコードされていることを確かめる
            
            service.UpdateHistory((HistoryType)2, new DateTime(2024, 10, 1).ToTimeZoneTokyo());
            var raw2 =  service.GetRawHistory((HistoryType)2);
            Assert.IsNotNull(raw2);
            Assert.AreEqual(raw2.Data, "\"2024-10-01T09:00:00\"");
            
            service.UpdateHistory((HistoryType)3, new CCDateTime(2024, 10, 1));
            var raw3 =  service.GetRawHistory((HistoryType)3);
            Assert.IsNotNull(raw3);
            Assert.AreEqual(raw3.Data, "\"2024-10-01T00:00:00\"");
            
            service.UpdateHistory((HistoryType)4, CCTimeSpan.FromTimeSpan(new TimeSpan(123, 45, 6)));
            var raw4 =  service.GetRawHistory((HistoryType)4);
            Assert.IsNotNull(raw4);
            _ = Convert.FromBase64String(raw4.Data.Trim('\"')); // Base64エンコードされていることを確かめる
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestRemoveHistory()
        {
            var ctx = InTestContext.Context;
            var service = ctx.GetService<HistoryService>();
            
            Assert.IsFalse(service.RemoveHistory((HistoryType)1));
            
            service.UpdateHistory((HistoryType)1, 123);
            Assert.IsTrue(service.RemoveHistory((HistoryType)1));
            Assert.IsFalse(service.RemoveHistory((HistoryType)1));
            Assert.IsFalse(service.RemoveHistory((HistoryType)2));
            
            ctx.Dispose();
        }
    }
}