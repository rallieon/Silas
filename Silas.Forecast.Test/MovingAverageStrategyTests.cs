using System;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class MovingAverageStrategyTests
    {
        private readonly MovingAverageStrategy _strategy = new MovingAverageStrategy();

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
            //Assert.AreEqual(99, _strategy.Forecast(data, 4, parameters));
        }

        [TestMethod]
        public void TestForecastInvalidPeriodNumber()
        {
            //Assert.AreEqual(0, _strategy.Forecast(data, 5, parameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include NumberOfWeights")]
        public void TestForecastMissingNumberOfWeights()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Weights = new[] { 0.5, 0.3, 0.2 };
            //Assert.AreEqual(0, _strategy.Forecast(data, 4, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include Weights")]
        public void TestForecastMissingWeights()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.NumberOfWeights = 3;
            //Assert.AreEqual(0, _strategy.Forecast(data, 4, testParameters));
        }
    }
}
