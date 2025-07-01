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
            NUnit.Framework.Assert.IsNotNull(service);
            NUnit.Framework.Assert.IsNotEmpty(service.Name);
        
        }
    }
}
