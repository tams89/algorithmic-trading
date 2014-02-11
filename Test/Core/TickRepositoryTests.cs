using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TradingCore.Model;
using TradingCore.Repository;

namespace Test.Core
{
    [TestFixture]
    public class TickRepositoryTests
    {
        [Test]
        public void GetTick_FromRepository()
        {
            var testTick = new Tick(
                Guid.NewGuid(), "IBM", DateTime.Today.Date, DateTime.Today.ToUniversalTime(),
                194.44M, 194.39M, 194.4M, 2650);

            var tickRepository = new Mock<IRepository<Tick>>();
            tickRepository.Setup(a => a.Get()).Returns(testTick);

            Assert.AreEqual(tickRepository.Object.Get(), testTick);
            tickRepository.Verify(x => x.Get(), Times.Once);
        }

        [Test]
        public void GetAllTicks_FromRepository()
        {
            var tickRepository = new Mock<IRepository<Tick>>();
            tickRepository.Setup(a => a.GetAll()).Returns(ticks);

            Assert.AreEqual(tickRepository.Object.GetAll(), ticks);
            tickRepository.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public void GetTick_FromReadOnlyRepository()
        {
            var testTick = new Tick(
                Guid.NewGuid(), "IBM", DateTime.Today.Date, DateTime.Today.ToUniversalTime(),
                194.44M, 194.39M, 194.4M, 2650);

            var tickRepository = new Mock<IReadOnlyRespository<Tick>>();
            tickRepository.Setup(a => a.Get()).Returns(testTick);

            Assert.AreEqual(tickRepository.Object.Get(), testTick);
            tickRepository.Verify(x => x.Get(), Times.Once);
        }

        [Test]
        public void GetAllTicks_FromReadOnlyRepository()
        {
            var tickRepository = new Mock<IReadOnlyRespository<Tick>>();
            tickRepository.Setup(a => a.GetAll()).Returns(ticks);

            Assert.AreEqual(tickRepository.Object.GetAll(), ticks);
            tickRepository.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public void GetTick_ByMember_FromReadOnlyRespository()
        {
            var tickRepository = new Mock<IReadOnlyRespository<Tick>>();
            var testData = ticks.Where(x => x.Symbol == "IBM");
            tickRepository.Setup(a => a.GetBy(x => x.Symbol == "IBM")).Returns(testData);

            Assert.AreEqual(tickRepository.Object.GetBy(x => x.Symbol == "IBM"), testData);
            tickRepository.Verify(x => x.GetBy(i => i.Symbol == "IBM"), Times.Once);
        }

        [Test]
        public void GetTick_NullData()
        {
            var tickRepository = new Mock<IReadOnlyRespository<Tick>>();
            tickRepository.Setup(a => a.Get());

            Tick testData = null;
            Assert.AreEqual(tickRepository.Object.Get(), testData);
            tickRepository.Verify(x => x.Get(), Times.Once);
        }

        [Test]
        public void GetAllTicks_NullData()
        {
            var tickRepository = new Mock<IReadOnlyRespository<Tick>>();
            tickRepository.Setup(a => a.GetAll());

            IEnumerable<Tick> testData = null;
            Assert.AreEqual(tickRepository.Object.GetAll(), testData);
            tickRepository.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public void GetTick_ByMember_FromReadOnlyRespository_NullData()
        {
            var tickRepository = new Mock<IReadOnlyRespository<Tick>>();
            IEnumerable<Tick> testData = null;
            tickRepository.Setup(a => a.GetBy(x => x.Symbol == "IBM")).Returns(testData);

            Assert.AreEqual(tickRepository.Object.GetBy(x => x.Symbol == "IBM"), testData);
            tickRepository.Verify(x => x.GetBy(i => i.Symbol == "IBM"), Times.Once);
        }

        /// <summary>
        /// Test tick data.
        /// </summary>
        private static readonly Tick[] ticks =
        {
            new Tick(new Guid("E5CB576D-BD03-4090-B49C-00000F8ED76E"), "IBM", DateTime.Parse("2013-08-05 00:00:00.000"),
                DateTime.Parse("13:54:00.0000000"), 194.44M, 194.39M, 194.4M, 2650),

            new Tick(new Guid("D0DBA151-2619-49DD-8D48-00000F9D438A"), "IBM", DateTime.Parse("2002-03-22 00:00:00.000"),
                DateTime.Parse("13:59:00.0000000"), 89.96M, 89.91M, 89.91M, 9750),

            new Tick(new Guid("5B357BD1-F4AE-409D-9E17-0000151B0871"), "IBM", DateTime.Parse("2006-10-23 00:00:00.000"),
                DateTime.Parse("14:53:00.0000000"), 81.07M, 81.05M, 81.05M, 20080),

            new Tick(new Guid("C13B8DE5-D28A-4A83-A651-000026EAF5BE"), "IBM", DateTime.Parse("2004-08-11 00:00:00.000"),
                DateTime.Parse("10:54:00.0000000"), 72.78M, 72.7M, 72.71M, 8640),

            new Tick(new Guid("A575D03D-81E8-4D07-A2E3-00002CE7838B"), "IBM", DateTime.Parse("2002-07-23 00:00:00.000"),
                DateTime.Parse("11:31:00.0000000"), 58.91M, 58.85M, 58.86M, 19820),

            new Tick(new Guid("672A65E4-D923-4863-A246-00002F70DB6E"), "IBM", DateTime.Parse("1998-07-10 00:00:00.000"),
                DateTime.Parse("15:04:00.0000000"), 49.66M, 49.61M, 49.61M, 3350),

            new Tick(new Guid("82D3922D-04FD-4915-AF47-0000459E1CA5"), "IBM", DateTime.Parse("2013-03-18 00:00:00.000"),
                DateTime.Parse("13:20:00.0000000"), 212.19M, 212.19M, 212.24M, 4910),

            new Tick(new Guid("12E02D16-C819-4A9D-AAB5-0000467CAA21"), "IBM", DateTime.Parse("2007-09-14 00:00:00.000"),
                DateTime.Parse("11:24:00.0000000"), 103.91M, 103.89M, 103.95M, 17590),

            new Tick(new Guid("5E872D1C-DFF0-417B-A893-0000553C2F3D"), "IBM", DateTime.Parse("2005-08-31 00:00:00.000"),
                DateTime.Parse("10:38:00.0000000"), 70.27M, 70.27M, 70.28M, 3430),

            new Tick(new Guid("F1336A10-62FA-45A5-8F59-000057419918"), "IBM", DateTime.Parse("2001-02-21 00:00:00.000"),
                DateTime.Parse("13:48:00.0000000"), 92.84M, 32.45M, 92.91M, 23170)
        };


    }
}
