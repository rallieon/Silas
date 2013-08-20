using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class NaieveStrategyTests
    {
        private readonly NaieveStrategy _strategy = new NaieveStrategy();
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
        }

        [TestMethod]
        public void TestForecastValueFirstPeriod()
        {
            Assert.AreEqual(10, _strategy.Forecast(_data, 1, null).ForecastValue);
        }

        [TestMethod]
        public void TestForecastValuePeriodInTheMiddle()
        {
            Assert.AreEqual(20, _strategy.Forecast(_data, 3, null).ForecastValue);
        }

        [TestMethod]
        public void TestForecastValueOnePeriodAhead()
        {
            Assert.AreEqual(50, _strategy.Forecast(_data, 6, null).ForecastValue);
        }

        [TestMethod]
        public void TestForecastErrorPeriodInTheMiddle()
        {
            Assert.AreEqual(10, _strategy.Forecast(_data, 3, null).Error);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalLowOnePeriodAhead()
        {
            Assert.AreEqual(50, _strategy.Forecast(_data, 6, null).ConfidenceIntervalLow);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalHighOnePeriodAhead()
        {
            Assert.AreEqual(50, _strategy.Forecast(_data, 6, null).ConfidenceIntervalHigh);
        }

        [TestMethod]
        public void TestForecastIsHoldout()
        {
            Assert.AreEqual(true, _strategy.Forecast(_data, 5, null).IsHoldout);
        }

        [TestMethod]
        public void TestForecastNotIsHoldout()
        {
            Assert.AreEqual(false, _strategy.Forecast(_data, 1, null).IsHoldout);
        }

        [TestMethod]
        public void TestForecastValueTwoPeriodAhead()
        {
            Assert.AreEqual(50, _strategy.Forecast(_data, 7, null).ForecastValue);
        }

        [TestMethod]
        public void TestForecastValueThreePeriodAhead()
        {
            Assert.AreEqual(50, _strategy.Forecast(_data, 8, null).ForecastValue);
        }

        [TestMethod]
        public void TestForecastZeroPeriod()
        {
            Assert.AreEqual(null, _strategy.Forecast(_data, 0, null));
        }
    }
}