using AppCore.Native;
using NUnit.Framework;

namespace Test.Integration
{
    public class TestNativeService
    {
        [Test]
        public void TestConstruct()
        {
            var service = new NativeService();
            Assert.IsNotNull(service);
            Assert.IsNotEmpty(service.Name);
        
        }
    }
}
