using Core.AutoMapper;
using NUnit.Framework;

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
