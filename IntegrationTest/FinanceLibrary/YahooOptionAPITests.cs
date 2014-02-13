using FinanceLibrary.YahooFinance.Option;
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
            var data = YahooOptionAPI.GetOptionsData(symbol);
            Assert.IsTrue(data.Any() == shouldPass);
        }
    }
}
