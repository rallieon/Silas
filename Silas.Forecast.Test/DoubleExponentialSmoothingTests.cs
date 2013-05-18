using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class DoubleExponentialSmoothingStrategyTests
    {
        private readonly DoubleExponentialSmoothingStrategy _strategy = new DoubleExponentialSmoothingStrategy();
        private dynamic _parameters;
        private const double _customEpsilon = 0.01;
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
                    new DataEntry {Id = 5, Period = 5, Value = 50},
                    new DataEntry {Id = 6, Period = 6, Value = 60},
                    new DataEntry {Id = 7, Period = 7, Value = 70},
                    new DataEntry {Id = 8, Period = 8, Value = 80},
                    new DataEntry {Id = 9, Period = 9, Value = 90},
                    new DataEntry {Id = 10, Period = 10, Value = 100},
                    new DataEntry {Id = 11, Period = 11, Value = 110},
                    new DataEntry {Id = 12, Period = 12, Value = 120},
                };

            _parameters = new ExpandoObject();
            _parameters.Alpha = 0.5;
            _parameters.Beta = 0.5;
        }

        [TestMethod]
        public void TestForecastValueFirstPeriod()
        {
            Assert.AreEqual(10, _strategy.Forecast(_data, 1, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueSecondPeriod()
        {
            Assert.AreEqual(10, _strategy.Forecast(_data, 2, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueOnePeriodAhead()
        {
            Assert.AreEqual(130, _strategy.Forecast(_data, 13, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueTwoPeriodAhead()
        {
            Assert.AreEqual(130, _strategy.Forecast(_data, 14, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueThreePeriodAhead()
        {
            Assert.AreEqual(130, _strategy.Forecast(_data, 15, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValuePeriodInTheMiddle()
        {
            Assert.AreEqual(50, _strategy.Forecast(_data, 5, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastErrorPeriodInTheMiddle()
        {
            Assert.AreEqual(0, _strategy.Forecast(_data, 5, _parameters).Error, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalLowOnePeriodAhead()
        {
            Assert.AreEqual(130, _strategy.Forecast(_data, 13, _parameters).ConfidenceIntervalLow, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalHighOnePeriodAhead()
        {
            Assert.AreEqual(130, _strategy.Forecast(_data, 13, _parameters).ConfidenceIntervalHigh, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastIsHoldout()
        {
            Assert.AreEqual(true, _strategy.Forecast(_data, 11, _parameters).IsHoldout);
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
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include Alpha")]
        public void TestForecastMissingAlpha()
        {
            dynamic testParameters = new ExpandoObject();
            Assert.AreEqual(0, _strategy.Forecast(_data, 13, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The strategy parameters must include Beta")]
        public void TestForecastMissingBeta()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            Assert.AreEqual(0, _strategy.Forecast(_data, 13, testParameters));
        }
    }
}
