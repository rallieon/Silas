using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class NaieveStrategyTests
    {
        private readonly NaieveStrategy _strategy = new NaieveStrategy();

        [TestMethod]
        public void TestForecastValidPeriodNumber()
        {
            var data = new[]{1,2,3};
            Assert.AreEqual(3, _strategy.Forecast(data, 4, null));
        }

        [TestMethod]
        public void TestForecastInvalidPeriodNumber()
        {
            var data = new[] { 1, 2, 3 };
            Assert.AreEqual(0, _strategy.Forecast(data, 5, null));
        }
    }
}
