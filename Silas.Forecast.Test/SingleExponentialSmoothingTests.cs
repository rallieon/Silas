using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class SingleExponentialSmoothingStrategyTests
    {
        private readonly SingleExponentialSmoothingStrategy _strategy = new SingleExponentialSmoothingStrategy();

        private int[] data;
        private dynamic parameters;

        [TestInitialize]
        public void Setup()
        {
            data = new[]{ 152, 163, 155, 168, 72, 161, 168, 179, 210 };
            parameters = new ExpandoObject();
            parameters.Alpha = 0.6;
        }

        [TestMethod]
        public void TestForecastValidPeriodNumber()
        {
            Assert.AreEqual(194, _strategy.Forecast(data, 10, parameters));
        }

        [TestMethod]
        public void TestForecastInvalidPeriodNumber()
        {
            Assert.AreEqual(0, _strategy.Forecast(data, 99, parameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include Alpha")]
        public void TestForecastMissingAlpha()
        {
            dynamic testParameters = new ExpandoObject();
            Assert.AreEqual(0, _strategy.Forecast(data, 10, testParameters));
        }
    }
}
