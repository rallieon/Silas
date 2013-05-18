using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class MovingAverageStrategyTests
    {
        private readonly MovingAverageStrategy _strategy = new MovingAverageStrategy();
        private dynamic _parameters;
        private IList<DataEntry> _data;

        [TestInitialize]
        public void Setup()
        {
            _data = new List<DataEntry>
                {
                    new DataEntry {Id = 1, Period = 1, Value = 10},
                    new DataEntry {Id = 2, Period = 2, Value = 20},
                    new DataEntry {Id = 3, Period = 3, Value = 30},
                    new DataEntry {Id = 4, Period = 4, Value = 40},
                    new DataEntry {Id = 5, Period = 5, Value = 50}
                };

            _parameters = new ExpandoObject();
            _parameters.PeriodCount = 2;
        }

        [TestMethod]
        public void TestForecastValueFirstPeriod()
        {
            Assert.AreEqual(10, _strategy.Forecast(_data, 1, _parameters).ForecastValue);
        }

        [TestMethod]
        public void TestForecastValuePeriodInTheMiddle()
        {
            Assert.AreEqual(15, _strategy.Forecast(_data, 3, _parameters).ForecastValue);
        }

        [TestMethod]
        public void TestForecastValueOnePeriodAhead()
        {
            Assert.AreEqual(45, _strategy.Forecast(_data, 6, _parameters).ForecastValue);
        }

        [TestMethod]
        public void TestForecastErrorPeriodInTheMiddle()
        {
            Assert.AreEqual(-15, _strategy.Forecast(_data, 3, _parameters).Error);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalLowOnePeriodAhead()
        {
            Assert.AreEqual(45, _strategy.Forecast(_data, 6, _parameters).ConfidenceIntervalLow);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalHighOnePeriodAhead()
        {
            Assert.AreEqual(45, _strategy.Forecast(_data, 6, _parameters).ConfidenceIntervalHigh);
        }

        [TestMethod]
        public void TestForecastIsHoldout()
        {
            Assert.AreEqual(true, _strategy.Forecast(_data, 5, _parameters).IsHoldout);
        }

        [TestMethod]
        public void TestForecastNotIsHoldout()
        {
            Assert.AreEqual(false, _strategy.Forecast(_data, 1, _parameters).IsHoldout);
        }

        [TestMethod]
        public void TestForecastValueTwoPeriodAhead()
        {
            Assert.AreEqual(45, _strategy.Forecast(_data, 7, _parameters).ForecastValue);
        }

        [TestMethod]
        public void TestForecastValueThreePeriodAhead()
        {
            Assert.AreEqual(45, _strategy.Forecast(_data, 8, _parameters).ForecastValue);
        }

        [TestMethod]
        public void TestForecastZeroPeriod()
        {
            Assert.AreEqual(null, _strategy.Forecast(_data, 0, _parameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include PeriodCount")]
        public void TestForecastMissingPeriodCount()
        {
            dynamic testParameters = new ExpandoObject();
            Assert.AreEqual(0, _strategy.Forecast(_data, 4, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The number of periods can not be greater than the number of entries.")]
        public void TestForecastInvalidPeriodCount()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.PeriodCount = 10;
            Assert.AreEqual(0, _strategy.Forecast(_data, 4, testParameters));
        }
    }
}
