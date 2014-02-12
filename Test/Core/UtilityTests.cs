using NUnit.Framework;
using System;
using TradingCore.Model;
using TradingCore.Utilities;

namespace Test.UnitTest.Core
{
    [TestFixture]
    public class UtilityTests
    {
        [TestCase("")]
        public void ExpressionChecker_NoException(string memberValue)
        {
            Assert.DoesNotThrow(() => ExpressionHelpers<Tick>.CheckMemberExpression(x => x.Symbol, memberValue));
        }

        [TestCase(10)]
        public void ExpressionChecker_NoException(int memberValue)
        {
            Assert.DoesNotThrow(() => ExpressionHelpers<Tick>.CheckMemberExpression(x => x.Volume, memberValue));
        }

        [TestCase(10.0)]
        public void ExpressionChecker_NoException(decimal memberValue)
        {
            Assert.DoesNotThrow(() => ExpressionHelpers<Tick>.CheckMemberExpression(x => x.Open, memberValue));
        }

        [TestCase(10)]
        public void ExpressionChecker_Exception(int memberValue)
        {
            Assert.Throws<ArgumentException>(() => ExpressionHelpers<Tick>.CheckMemberExpression(x => x.Symbol, memberValue));
        }

        [TestCase("")]
        public void ExpressionChecker_Exception(string memberValue)
        {
            Assert.Throws<ArgumentException>(() => ExpressionHelpers<Tick>.CheckMemberExpression(x => x.Open, memberValue));
        }
    }
}
