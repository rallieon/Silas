using System;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class WeightedAverageStrategyTests
    {
        private readonly WeightedAverageStrategy _strategy = new WeightedAverageStrategy();

        private int[] data;
        private dynamic parameters;

        [TestInitialize]
        public void Setup()
        {
            data = new[] { 100, 30, 200 };
            parameters = new ExpandoObject();
            parameters.NumberOfWeights = 3;
            parameters.Weights = new[] { 0.5, 0.3, 0.2 };
        }

        [TestMethod]
        public void TestForecastValidPeriodNumber()
        {
            Assert.AreEqual(99, _strategy.Forecast(data, 4, parameters));
        }

        [TestMethod]
        public void TestForecastInvalidPeriodNumber()
        {
            Assert.AreEqual(0, _strategy.Forecast(data, 5, null));
        }
    }
}
