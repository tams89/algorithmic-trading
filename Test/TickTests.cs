using Moq;
using NUnit.Framework;
using TradingCore.Model;
using TradingCore.Repository;

namespace Test
{
    [TestFixture]
    public class TickTests
    {
        [Test]
        public void GetTick()
        {
            var testTick = new Tick();

            var tickRepository = new Mock<IRepository<Tick>>()
                .Setup(a => a.Get()).Returns();
        }
    }
}
