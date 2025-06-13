using System;
using Domain.Entity;
using Domain.Enum;
using NUnit.Framework;

namespace Test.Unit
{
    public class TestHistoryRepository
    {
        [Test]
        public void TestUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.HistoryRepo;
            
            var time = new DateTime(2024, 10, 1).ToLocalTime();
            Assert.IsFalse(repo.Update(new HistoryContainer((HistoryType)1, "", time)));

            repo.InsertUpdate(new HistoryContainer((HistoryType)1, "1", time));
            var target = new HistoryContainer((HistoryType)1, "12", time);
            Assert.IsTrue(repo.Update(new HistoryContainer((HistoryType)1, "12", time)));
            
            Assert.AreEqual(target, repo.Get((HistoryType)1));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestInsertUpdate()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.HistoryRepo;
            
            var time = new DateTime(2024, 10, 1).ToLocalTime();
            Assert.IsTrue(repo.InsertUpdate(new HistoryContainer((HistoryType)1, "", time)));

            var target = new HistoryContainer((HistoryType)1, "1", time);
            Assert.IsFalse(repo.InsertUpdate(target));
            Assert.AreEqual(target, repo.Get((HistoryType)1));
            
            Assert.IsTrue(repo.InsertUpdate(new HistoryContainer((HistoryType)2, "2", time)));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestRemove()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.HistoryRepo;
            
            Assert.IsFalse(repo.Remove((HistoryType)1));

            var time = new DateTime(2024, 10, 1).ToLocalTime();
            repo.InsertUpdate(new HistoryContainer((HistoryType)1, "1", time));
            Assert.IsTrue(repo.Remove((HistoryType)1));
            Assert.IsFalse(repo.Remove((HistoryType)1));

            repo.InsertUpdate(new HistoryContainer((HistoryType)2, "2", time));
            Assert.IsTrue(repo.Remove((HistoryType)2));
            
            ctx.Dispose();
        }
        
        [Test]
        public void TestGet()
        {
            var ctx = InTestContext.Context;
            var repo = ctx.HistoryRepo;
            
            Assert.IsNull(repo.Get((HistoryType)1));
            
            var time = new DateTime(2024, 10, 1).ToLocalTime();
            repo.InsertUpdate(new HistoryContainer((HistoryType)1, "", time));
            Assert.IsNotNull(repo.Get((HistoryType)1));
            
            var target = new HistoryContainer((HistoryType)1, "1", time);
            repo.Update(target);
            Assert.AreEqual(target, repo.Get((HistoryType)1));
            
            repo.Remove((HistoryType)1);
            Assert.IsNull(repo.Get((HistoryType)1));
            
            ctx.Dispose();
        }
    }
}