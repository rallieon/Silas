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
    public class DoubleExponentialSmoothingStrategyTests
    {
        private readonly DoubleExponentialSmoothingStrategy _strategy = new DoubleExponentialSmoothingStrategy();

        private int[] data;
        private dynamic parameters;

        [TestInitialize]
        public void Setup()
        {
            data = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120 };
            parameters = new ExpandoObject();
            parameters.Alpha = 0.5;
            parameters.Beta = 0.5;
        }

        [TestMethod]
        public void TestForecastValidPeriodNumber()
        {
            Assert.AreEqual(130, _strategy.Forecast(data, 13, parameters));
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
            Assert.AreEqual(0, _strategy.Forecast(data, 13, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include Beta")]
        public void TestForecastMissingBeta()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            Assert.AreEqual(0, _strategy.Forecast(data, 13, testParameters));
        }
    }
}
