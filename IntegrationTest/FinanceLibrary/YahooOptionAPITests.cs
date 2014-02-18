using FinanceLibrary;
using FinanceLibrary.YahooFinanceAPI;
using NUnit.Framework;
using System.Linq;

namespace Test.IntegrationTest.FinanceLibrary
{
    [TestFixture]
    public class YahooOptionAPITests
    {
        [TestCase("MSFT", true)]
        [TestCase("GOOG", true)]
        [TestCase("", false)]
        public void GetOptionData(string symbol, bool shouldPass)
        {
            var service = new Option.GetOptionTableService() as IOptionService;
            var data = service.GetOptionTable(symbol);
            Assert.IsTrue(data.Any() == shouldPass);
        }
    }
}
