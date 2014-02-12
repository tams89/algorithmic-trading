using NUnit.Framework;
using System;
using System.Linq;
using TradingCore.Model;
using TradingCore.Repository;
using TradingCore.Repository.Interface;

namespace Test.IntegrationTest.Core
{
    [TestFixture]
    public class GenericRepositoryTests
    {
        private readonly IReadOnlyRespository<Tick> tickRepository = new Repository<Tick>();
        private readonly IRepository<HistoricalStock> historicalStockRepository = new Repository<HistoricalStock>();

        [TestCase("E5CB576D-BD03-4090-B49C-00000F8ED76E", true)] // In DB
        [TestCase("E5CB589D-BD03-4090-B49C-00000F8ED76E", false)] // Not in DB
        public void GetSingleEntity(string id, bool shouldPass)
        {
            var data = tickRepository.Get(Guid.Parse(id));
            Assert.IsTrue(data != null == shouldPass);
        }

        [Test]
        public void GetAllEntities()
        {
            var data = tickRepository.GetAll();
            Assert.IsNotEmpty(data);
        }

        [TestCase("IBM", true)]
        [TestCase("", false)]
        [TestCase("ergeg", false)]
        public void GetBySymbol(string symbol, bool shouldPass)
        {
            var data = tickRepository.GetBy(x => x.Symbol, symbol);
            Assert.IsTrue(data.Any() == shouldPass);
        }

        [TestCase(1)]
        public void GetBySymbol(int memberValue)
        {
            Assert.Catch<ArgumentException>(() => tickRepository.GetBy(x => x.Symbol, memberValue));
        }

        [TestCase(651.65)]
        public void GetBySymbol(decimal memberValue)
        {
            Assert.Catch<ArgumentException>(() => tickRepository.GetBy(x => x.Symbol, memberValue));
        }

        [TestCase(9999999, false)]
        [TestCase(194.44, true)]
        public void GetByOpen(decimal memberValue, bool shouldPass)
        {
            var data = tickRepository.GetBy(x => x.Open, memberValue);
            Assert.IsTrue(data.Any() == shouldPass);
        }

        [Test]
        public void GetByNull()
        {
            Assert.Catch<ArgumentNullException>(() => tickRepository.GetBy(x => x, null));
        }

        [Test]
        public void CreateReadDelete_HistoricalStockData()
        {
            var id = Guid.Parse("73F8252A-01A8-4710-97C4-A2D00108271E");
            var historicalStock = new HistoricalStock(id, "TEST", "TEST", DateTime.Now, 100M, 104M, 98M, 94.5M, 4500);
            Assert.DoesNotThrow(() => historicalStockRepository.Insert(historicalStock));

            // Verify insertion by performing a select
            var selectHistoricalStock = historicalStockRepository.GetBy(x => x.HistoricalStockId, id).Single();
            Assert.AreEqual(historicalStock.HistoricalStockId, selectHistoricalStock.HistoricalStockId);

            // Remove test data
            Assert.DoesNotThrow(() => historicalStockRepository.Delete(historicalStock));
        }

        [Test]
        public void CreateUpdateRead_HistoricalStockData()
        {
            // Create
            var id = Guid.Parse("73F8252A-01A8-4710-97C4-A2D00108271E");
            var historicalStock = new HistoricalStock(id, "TEST", "TEST", DateTime.Now, 100M, 104M, 98M, 94.5M, 4500);
            Assert.DoesNotThrow(() => historicalStockRepository.Insert(historicalStock));

            // Update
            historicalStock.Open = 1000M;
            Assert.DoesNotThrow(() => historicalStockRepository.Update(historicalStock));

            // Read, check if updated to new value.
            var selectHistoricalStock = historicalStockRepository.GetBy(x => x.HistoricalStockId, id).Single();
            Assert.AreEqual(1000M, selectHistoricalStock.Open);

            // Delete
            Assert.DoesNotThrow(() => historicalStockRepository.Delete(historicalStock));
        }
    }
}