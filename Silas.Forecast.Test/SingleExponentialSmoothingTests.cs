using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class SingleExponentialSmoothingStrategyTests
    {
        private const double _customEpsilon = 0.01;
        private readonly SingleExponentialSmoothingStrategy _strategy = new SingleExponentialSmoothingStrategy();
        private IList<DataEntry> _data;
        private dynamic _parameters;

        [TestInitialize]
        public void Setup()
        {
            _data = new List<DataEntry>
                {
                    new DataEntry {Id = 1, Period = 1, Value = 152},
                    new DataEntry {Id = 2, Period = 2, Value = 163},
                    new DataEntry {Id = 3, Period = 3, Value = 155},
                    new DataEntry {Id = 4, Period = 4, Value = 168},
                    new DataEntry {Id = 5, Period = 5, Value = 72},
                    new DataEntry {Id = 6, Period = 6, Value = 161},
                    new DataEntry {Id = 7, Period = 7, Value = 168},
                    new DataEntry {Id = 8, Period = 8, Value = 179},
                    new DataEntry {Id = 9, Period = 9, Value = 210}
                };

            _parameters = new ExpandoObject();
            _parameters.Alpha = 0.6;
        }

        [TestMethod]
        public void TestForecastValueFirstPeriod()
        {
            Assert.AreEqual(152, _strategy.Forecast(_data, 1, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueSecondPeriod()
        {
            Assert.AreEqual(152, _strategy.Forecast(_data, 2, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValuePeriodInTheMiddle()
        {
            Assert.AreEqual(163.37, _strategy.Forecast(_data, 5, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueOnePeriodAhead()
        {
            Assert.AreEqual(194.04, _strategy.Forecast(_data, 10, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueTwoPeriodAhead()
        {
            Assert.AreEqual(194.04, _strategy.Forecast(_data, 11, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueThreePeriodAhead()
        {
            Assert.AreEqual(194.04, _strategy.Forecast(_data, 12, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastErrorPeriodInTheMiddle()
        {
            Assert.AreEqual(-91.37, _strategy.Forecast(_data, 5, _parameters).Error, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalLowOnePeriodAhead()
        {
            Assert.AreEqual(194.04, _strategy.Forecast(_data, 10, _parameters).ConfidenceIntervalLow, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalHighOnePeriodAhead()
        {
            Assert.AreEqual(194.04, _strategy.Forecast(_data, 10, _parameters).ConfidenceIntervalHigh, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastIsHoldout()
        {
            Assert.AreEqual(true, _strategy.Forecast(_data, 9, _parameters).IsHoldout);
        }

        [TestMethod]
        public void TestForecastNotIsHoldout()
        {
            Assert.AreEqual(false, _strategy.Forecast(_data, 1, _parameters).IsHoldout);
        }

        [TestMethod]
        public void TestForecastZeroPeriod()
        {
            Assert.AreEqual(null, _strategy.Forecast(_data, 0, _parameters));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException), "The strategy parameters must include Alpha")]
        public void TestForecastMissingAlpha()
        {
            dynamic testParameters = new ExpandoObject();
            Assert.AreEqual(0, _strategy.Forecast(_data, 10, testParameters), _customEpsilon);
        }
    }
}