using Core.AutoMapper;
using NUnit.Framework;

namespace Test.UnitTest
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
