using NUnit.Framework;
using TradingCore.AutoMapper;

namespace Test.UnitTest.Core
{
    [TestFixture]
    public class AutoMapperTests
    {
        [Test]
        public void Configures_Ok()
        {
            Assert.DoesNotThrow(AutoMapperConfig.Configure);
        }
    }
}
