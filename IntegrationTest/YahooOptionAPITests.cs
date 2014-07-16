using System.Linq;
using Algorithm.Core;
using Algorithm.Core.YahooFinanceService;
using NUnit.Framework;

namespace Test.IntegrationTest
{
    [TestFixture]
    public class YahooOptionApiTests
    {
        [TestCase("MSFT", true)]
        [TestCase("GOOG", false)]
        [TestCase("", false)]
        public void GetOptionData(string symbol, bool shouldPass)
        {
            var service = new Option.GetOptionTableService() as Interfaces.IOptionService;
            var data = service.GetOptionTable(symbol);
            Assert.IsTrue(data.Any() == shouldPass);
        }
    }
}
