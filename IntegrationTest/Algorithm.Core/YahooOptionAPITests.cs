using Algorithm.Core;
using Algorithm.Core.YahooFinanceService;
using NUnit.Framework;
using System.Linq;

namespace Test.IntegrationTest.Algorithm.Core
{
    [TestFixture]
    public class YahooOptionApiTests
    {
        [TestCase("MSFT", true)]
        [TestCase("GOOG", true)]
        [TestCase("", false)]
        public void GetOptionData(string symbol, bool shouldPass)
        {
            var service = new Option.GetOptionTableService() as Interfaces.IOptionService;
            var data = service.GetOptionTable(symbol);
            Assert.IsTrue(data.Any() == shouldPass);
        }
    }
}
