using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Test
{
    [TestClass]
    public class TripleExponentialSmoothingTests
    {
        private const double _customEpsilon = 0.001;
        private readonly TripleExponentialSmoothingStrategy _strategy = new TripleExponentialSmoothingStrategy();
        private IList<DataEntry> _data;
        private dynamic _parameters;

        [TestInitialize]
        public void Setup()
        {
            InitData();
            _parameters = new ExpandoObject();
            _parameters.Alpha = 0.0223259097162289;
            _parameters.Beta = 0.149874116913357;
            _parameters.Gamma = 0.12791654697988;
            _parameters.PeriodsPerSeason = 12;
            _parameters.SeasonsForRegression = 3;
        }

        [TestMethod]
        public void TestForecastValueLastRealPeriod()
        {
            Assert.AreEqual(2222.887, _strategy.Forecast(_data, 444, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueOnePeriodAhead()
        {
            Assert.AreEqual(2510.335, _strategy.Forecast(_data, 445, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueTwoPeriodAhead()
        {
            Assert.AreEqual(2170.356, _strategy.Forecast(_data, 446, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueThreePeriodAhead()
        {
            Assert.AreEqual(1954.961, _strategy.Forecast(_data, 447, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValueFourPeriodAhead()
        {
            Assert.AreEqual(1520.253, _strategy.Forecast(_data, 448, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastValuePeriodInTheMiddle()
        {
            Assert.AreEqual(1677.080, _strategy.Forecast(_data, 134, _parameters).ForecastValue, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastErrorPeriodInTheMiddle()
        {
            Assert.AreEqual(-1.224, _strategy.Forecast(_data, 134, _parameters).Error, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalLowOnePeriodAhead()
        {
            Assert.AreEqual(2510.335, _strategy.Forecast(_data, 445, _parameters).ConfidenceIntervalLow, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastConfidenceIntervalHighOnePeriodAhead()
        {
            Assert.AreEqual(2510.335, _strategy.Forecast(_data, 445, _parameters).ConfidenceIntervalHigh, _customEpsilon);
        }

        [TestMethod]
        public void TestForecastIsHoldout()
        {
            Assert.AreEqual(true, _strategy.Forecast(_data, 440, _parameters).IsHoldout);
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
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TestForecastMissingAlpha()
        {
            dynamic testParameters = new ExpandoObject();
            Assert.AreEqual(0, _strategy.Forecast(_data, 445, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TestForecastMissingBeta()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            Assert.AreEqual(0, _strategy.Forecast(_data, 445, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TestForecastMissingGamma()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            testParameters.Beta = 0.32;
            Assert.AreEqual(0, _strategy.Forecast(_data, 445, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TestForecastMissingPeriodsPerSeason()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            testParameters.Beta = 0.32;
            testParameters.Gamma = 0.32;
            Assert.AreEqual(0, _strategy.Forecast(_data, 445, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void TestForecastMissingSeasonsForRegression()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            testParameters.Beta = 0.32;
            testParameters.Gamma = 0.32;
            testParameters.PeriodsPerSeason = 12;
            Assert.AreEqual(0, _strategy.Forecast(_data, 445, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException),
            "You need at least three full seasons of data to build a data model.")]
        public void TestForecastInvalidPeriodsPerSeason()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            testParameters.Beta = 0.32;
            testParameters.Gamma = 0.32;
            testParameters.PeriodsPerSeason = 200;
            testParameters.SeasonsForRegression = 200;
            Assert.AreEqual(0, _strategy.Forecast(_data, 445, testParameters));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException), "You cannot forecast more than one future ahead.")]
        public void TestForecastInvalidPeriodMoreThanOneSeasonAhead()
        {
            dynamic testParameters = new ExpandoObject();
            testParameters.Alpha = 0.34;
            testParameters.Beta = 0.32;
            testParameters.Gamma = 0.32;
            testParameters.PeriodsPerSeason = 12;
            testParameters.SeasonsForRegression = 3;
            Assert.AreEqual(0, _strategy.Forecast(_data, 460, testParameters));
        }

        private void InitData()
        {
            _data = new List<DataEntry>
                {
                    new DataEntry {Id = 1, Period = 1, Value = 1932.187},
                    new DataEntry {Id = 2, Period = 2, Value = 1687.255},
                    new DataEntry {Id = 3, Period = 3, Value = 1497.067},
                    new DataEntry {Id = 4, Period = 4, Value = 1177.661},
                    new DataEntry {Id = 5, Period = 5, Value = 1015.008},
                    new DataEntry {Id = 6, Period = 6, Value = 933.663},
                    new DataEntry {Id = 7, Period = 7, Value = 981.183},
                    new DataEntry {Id = 8, Period = 8, Value = 1018.978},
                    new DataEntry {Id = 9, Period = 9, Value = 956.55},
                    new DataEntry {Id = 10, Period = 10, Value = 991.668},
                    new DataEntry {Id = 11, Period = 11, Value = 1197.249},
                    new DataEntry {Id = 12, Period = 12, Value = 1507.395},
                    new DataEntry {Id = 13, Period = 13, Value = 1823.499},
                    new DataEntry {Id = 14, Period = 14, Value = 1562.936},
                    new DataEntry {Id = 15, Period = 15, Value = 1436.363},
                    new DataEntry {Id = 16, Period = 16, Value = 1215.416},
                    new DataEntry {Id = 17, Period = 17, Value = 1010.945},
                    new DataEntry {Id = 18, Period = 18, Value = 936.124},
                    new DataEntry {Id = 19, Period = 19, Value = 1013.51},
                    new DataEntry {Id = 20, Period = 20, Value = 987.592},
                    new DataEntry {Id = 21, Period = 21, Value = 919.805},
                    new DataEntry {Id = 22, Period = 22, Value = 999.326},
                    new DataEntry {Id = 23, Period = 23, Value = 1141.133},
                    new DataEntry {Id = 24, Period = 24, Value = 1607.437},
                    new DataEntry {Id = 25, Period = 25, Value = 1758.226},
                    new DataEntry {Id = 26, Period = 26, Value = 1615.051},
                    new DataEntry {Id = 27, Period = 27, Value = 1538.328},
                    new DataEntry {Id = 28, Period = 28, Value = 1366.55},
                    new DataEntry {Id = 29, Period = 29, Value = 1034.664},
                    new DataEntry {Id = 30, Period = 30, Value = 936.766},
                    new DataEntry {Id = 31, Period = 31, Value = 1003.672},
                    new DataEntry {Id = 32, Period = 32, Value = 1012.72},
                    new DataEntry {Id = 33, Period = 33, Value = 926.799},
                    new DataEntry {Id = 34, Period = 34, Value = 954.616},
                    new DataEntry {Id = 35, Period = 35, Value = 1065.259},
                    new DataEntry {Id = 36, Period = 36, Value = 1598.15},
                    new DataEntry {Id = 37, Period = 37, Value = 1986.659},
                    new DataEntry {Id = 38, Period = 38, Value = 1674.363},
                    new DataEntry {Id = 39, Period = 39, Value = 1434.389},
                    new DataEntry {Id = 40, Period = 40, Value = 1163.868},
                    new DataEntry {Id = 41, Period = 41, Value = 1009.166},
                    new DataEntry {Id = 42, Period = 42, Value = 964.724},
                    new DataEntry {Id = 43, Period = 43, Value = 1006.965},
                    new DataEntry {Id = 44, Period = 44, Value = 1002.932},
                    new DataEntry {Id = 45, Period = 45, Value = 923.155},
                    new DataEntry {Id = 46, Period = 46, Value = 1005.253},
                    new DataEntry {Id = 47, Period = 47, Value = 1372.664},
                    new DataEntry {Id = 48, Period = 48, Value = 1863.448},
                    new DataEntry {Id = 49, Period = 49, Value = 2239.681},
                    new DataEntry {Id = 50, Period = 50, Value = 1869.527},
                    new DataEntry {Id = 51, Period = 51, Value = 1441.002},
                    new DataEntry {Id = 52, Period = 52, Value = 1132.077},
                    new DataEntry {Id = 53, Period = 53, Value = 979.115},
                    new DataEntry {Id = 54, Period = 54, Value = 971.294},
                    new DataEntry {Id = 55, Period = 55, Value = 1084.85},
                    new DataEntry {Id = 56, Period = 56, Value = 1065.597},
                    new DataEntry {Id = 57, Period = 57, Value = 974.452},
                    new DataEntry {Id = 58, Period = 58, Value = 1018.153},
                    new DataEntry {Id = 59, Period = 59, Value = 1173.632},
                    new DataEntry {Id = 60, Period = 60, Value = 1708.154},
                    new DataEntry {Id = 61, Period = 61, Value = 2000.053},
                    new DataEntry {Id = 62, Period = 62, Value = 1924.551},
                    new DataEntry {Id = 63, Period = 63, Value = 1696.087},
                    new DataEntry {Id = 64, Period = 64, Value = 1206.671},
                    new DataEntry {Id = 65, Period = 65, Value = 1081.209},
                    new DataEntry {Id = 66, Period = 66, Value = 1006.572},
                    new DataEntry {Id = 67, Period = 67, Value = 1091.233},
                    new DataEntry {Id = 68, Period = 68, Value = 1118.362},
                    new DataEntry {Id = 69, Period = 69, Value = 1037.052},
                    new DataEntry {Id = 70, Period = 70, Value = 1050.203},
                    new DataEntry {Id = 71, Period = 71, Value = 1223.173},
                    new DataEntry {Id = 72, Period = 72, Value = 1692.701},
                    new DataEntry {Id = 73, Period = 73, Value = 2087.712},
                    new DataEntry {Id = 74, Period = 74, Value = 1987.811},
                    new DataEntry {Id = 75, Period = 75, Value = 1608.938},
                    new DataEntry {Id = 76, Period = 76, Value = 1240.043},
                    new DataEntry {Id = 77, Period = 77, Value = 1030.974},
                    new DataEntry {Id = 78, Period = 78, Value = 956.854},
                    new DataEntry {Id = 79, Period = 79, Value = 1038.818},
                    new DataEntry {Id = 80, Period = 80, Value = 1087.877},
                    new DataEntry {Id = 81, Period = 81, Value = 973.422},
                    new DataEntry {Id = 82, Period = 82, Value = 1016.045},
                    new DataEntry {Id = 83, Period = 83, Value = 1205.234},
                    new DataEntry {Id = 84, Period = 84, Value = 1576.952},
                    new DataEntry {Id = 85, Period = 85, Value = 1826.947},
                    new DataEntry {Id = 86, Period = 86, Value = 1779.042},
                    new DataEntry {Id = 87, Period = 87, Value = 1603.835},
                    new DataEntry {Id = 88, Period = 88, Value = 1209.233},
                    new DataEntry {Id = 89, Period = 89, Value = 1016.289},
                    new DataEntry {Id = 90, Period = 90, Value = 1003.445},
                    new DataEntry {Id = 91, Period = 91, Value = 1163.408},
                    new DataEntry {Id = 92, Period = 92, Value = 1168.734},
                    new DataEntry {Id = 93, Period = 93, Value = 1041.657},
                    new DataEntry {Id = 94, Period = 94, Value = 1043.756},
                    new DataEntry {Id = 95, Period = 95, Value = 1226.25},
                    new DataEntry {Id = 96, Period = 96, Value = 1670.298},
                    new DataEntry {Id = 97, Period = 97, Value = 2014.965},
                    new DataEntry {Id = 98, Period = 98, Value = 1683.398},
                    new DataEntry {Id = 99, Period = 99, Value = 1440.933},
                    new DataEntry {Id = 100, Period = 100, Value = 1108.806},
                    new DataEntry {Id = 101, Period = 101, Value = 996.568},
                    new DataEntry {Id = 102, Period = 102, Value = 1011.218},
                    new DataEntry {Id = 103, Period = 103, Value = 1125.714},
                    new DataEntry {Id = 104, Period = 104, Value = 1086.1},
                    new DataEntry {Id = 105, Period = 105, Value = 951.551},
                    new DataEntry {Id = 106, Period = 106, Value = 1049.961},
                    new DataEntry {Id = 107, Period = 107, Value = 1180.761},
                    new DataEntry {Id = 108, Period = 108, Value = 1605.626},
                    new DataEntry {Id = 109, Period = 109, Value = 2037.891},
                    new DataEntry {Id = 110, Period = 110, Value = 1742.925},
                    new DataEntry {Id = 111, Period = 111, Value = 1533.826},
                    new DataEntry {Id = 112, Period = 112, Value = 1294.481},
                    new DataEntry {Id = 113, Period = 113, Value = 1009.215},
                    new DataEntry {Id = 114, Period = 114, Value = 968.998},
                    new DataEntry {Id = 115, Period = 115, Value = 1104.857},
                    new DataEntry {Id = 116, Period = 116, Value = 1099.316},
                    new DataEntry {Id = 117, Period = 117, Value = 1022.053},
                    new DataEntry {Id = 118, Period = 118, Value = 1005.641},
                    new DataEntry {Id = 119, Period = 119, Value = 1168.342},
                    new DataEntry {Id = 120, Period = 120, Value = 1539.935},
                    new DataEntry {Id = 121, Period = 121, Value = 1922.144},
                    new DataEntry {Id = 122, Period = 122, Value = 1580.016},
                    new DataEntry {Id = 123, Period = 123, Value = 1490.168},
                    new DataEntry {Id = 124, Period = 124, Value = 1273.99},
                    new DataEntry {Id = 125, Period = 125, Value = 1034.261},
                    new DataEntry {Id = 126, Period = 126, Value = 978.628},
                    new DataEntry {Id = 127, Period = 127, Value = 1141.32},
                    new DataEntry {Id = 128, Period = 128, Value = 1213.98},
                    new DataEntry {Id = 129, Period = 129, Value = 1056.284},
                    new DataEntry {Id = 130, Period = 130, Value = 976.307},
                    new DataEntry {Id = 131, Period = 131, Value = 1130.908},
                    new DataEntry {Id = 132, Period = 132, Value = 1625.005},
                    new DataEntry {Id = 133, Period = 133, Value = 2129.572},
                    new DataEntry {Id = 134, Period = 134, Value = 1675.856},
                    new DataEntry {Id = 135, Period = 135, Value = 1589.806},
                    new DataEntry {Id = 136, Period = 136, Value = 1291.656},
                    new DataEntry {Id = 137, Period = 137, Value = 1108.134},
                    new DataEntry {Id = 138, Period = 138, Value = 1052.346},
                    new DataEntry {Id = 139, Period = 139, Value = 1132.58},
                    new DataEntry {Id = 140, Period = 140, Value = 1168.674},
                    new DataEntry {Id = 141, Period = 141, Value = 1028.098},
                    new DataEntry {Id = 142, Period = 142, Value = 1008.054},
                    new DataEntry {Id = 143, Period = 143, Value = 1187.911},
                    new DataEntry {Id = 144, Period = 144, Value = 1582.196},
                    new DataEntry {Id = 145, Period = 145, Value = 1953.435},
                    new DataEntry {Id = 146, Period = 146, Value = 1913.52},
                    new DataEntry {Id = 147, Period = 147, Value = 1516.21},
                    new DataEntry {Id = 148, Period = 148, Value = 1206.396},
                    new DataEntry {Id = 149, Period = 149, Value = 1014.664},
                    new DataEntry {Id = 150, Period = 150, Value = 1035.519},
                    new DataEntry {Id = 151, Period = 151, Value = 1149.519},
                    new DataEntry {Id = 152, Period = 152, Value = 1156.849},
                    new DataEntry {Id = 153, Period = 153, Value = 1064.453},
                    new DataEntry {Id = 154, Period = 154, Value = 1055.261},
                    new DataEntry {Id = 155, Period = 155, Value = 1180.795},
                    new DataEntry {Id = 156, Period = 156, Value = 1795.12},
                    new DataEntry {Id = 157, Period = 157, Value = 1981.74},
                    new DataEntry {Id = 158, Period = 158, Value = 1684.176},
                    new DataEntry {Id = 159, Period = 159, Value = 1539.834},
                    new DataEntry {Id = 160, Period = 160, Value = 1180.938},
                    new DataEntry {Id = 161, Period = 161, Value = 1057.117},
                    new DataEntry {Id = 162, Period = 162, Value = 1057.373},
                    new DataEntry {Id = 163, Period = 163, Value = 1249.832},
                    new DataEntry {Id = 164, Period = 164, Value = 1187.564},
                    new DataEntry {Id = 165, Period = 165, Value = 1049.093},
                    new DataEntry {Id = 166, Period = 166, Value = 1078.56},
                    new DataEntry {Id = 167, Period = 167, Value = 1229.547},
                    new DataEntry {Id = 168, Period = 168, Value = 1675.117},
                    new DataEntry {Id = 169, Period = 169, Value = 1931.481},
                    new DataEntry {Id = 170, Period = 170, Value = 1681.156},
                    new DataEntry {Id = 171, Period = 171, Value = 1554.99},
                    new DataEntry {Id = 172, Period = 172, Value = 1250.069},
                    new DataEntry {Id = 173, Period = 173, Value = 1079.592},
                    new DataEntry {Id = 174, Period = 174, Value = 1112.209},
                    new DataEntry {Id = 175, Period = 175, Value = 1268.777},
                    new DataEntry {Id = 176, Period = 176, Value = 1271.368},
                    new DataEntry {Id = 177, Period = 177, Value = 1079.85},
                    new DataEntry {Id = 178, Period = 178, Value = 1104.985},
                    new DataEntry {Id = 179, Period = 179, Value = 1243.133},
                    new DataEntry {Id = 180, Period = 180, Value = 1686.844},
                    new DataEntry {Id = 181, Period = 181, Value = 2133.93},
                    new DataEntry {Id = 182, Period = 182, Value = 1859.792},
                    new DataEntry {Id = 183, Period = 183, Value = 1630.123},
                    new DataEntry {Id = 184, Period = 184, Value = 1260.376},
                    new DataEntry {Id = 185, Period = 185, Value = 1108.817},
                    new DataEntry {Id = 186, Period = 186, Value = 1121.331},
                    new DataEntry {Id = 187, Period = 187, Value = 1312.656},
                    new DataEntry {Id = 188, Period = 188, Value = 1373.343},
                    new DataEntry {Id = 189, Period = 189, Value = 1116.235},
                    new DataEntry {Id = 190, Period = 190, Value = 1134.134},
                    new DataEntry {Id = 191, Period = 191, Value = 1317.127},
                    new DataEntry {Id = 192, Period = 192, Value = 1765.873},
                    new DataEntry {Id = 193, Period = 193, Value = 1974.55},
                    new DataEntry {Id = 194, Period = 194, Value = 1848.713},
                    new DataEntry {Id = 195, Period = 195, Value = 1771.579},
                    new DataEntry {Id = 196, Period = 196, Value = 1339.39},
                    new DataEntry {Id = 197, Period = 197, Value = 1174.052},
                    new DataEntry {Id = 198, Period = 198, Value = 1174.373},
                    new DataEntry {Id = 199, Period = 199, Value = 1319.846},
                    new DataEntry {Id = 200, Period = 200, Value = 1319.206},
                    new DataEntry {Id = 201, Period = 201, Value = 1180.539},
                    new DataEntry {Id = 202, Period = 202, Value = 1183.342},
                    new DataEntry {Id = 203, Period = 203, Value = 1382.35},
                    new DataEntry {Id = 204, Period = 204, Value = 2118.428},
                    new DataEntry {Id = 205, Period = 205, Value = 2020.627},
                    new DataEntry {Id = 206, Period = 206, Value = 1641.38},
                    new DataEntry {Id = 207, Period = 207, Value = 1559.608},
                    new DataEntry {Id = 208, Period = 208, Value = 1299.688},
                    new DataEntry {Id = 209, Period = 209, Value = 1142.782},
                    new DataEntry {Id = 210, Period = 210, Value = 1194.847},
                    new DataEntry {Id = 211, Period = 211, Value = 1334.278},
                    new DataEntry {Id = 212, Period = 212, Value = 1322.712},
                    new DataEntry {Id = 213, Period = 213, Value = 1214.41},
                    new DataEntry {Id = 214, Period = 214, Value = 1161.169},
                    new DataEntry {Id = 215, Period = 215, Value = 1292.48},
                    new DataEntry {Id = 216, Period = 216, Value = 1753.127},
                    new DataEntry {Id = 217, Period = 217, Value = 2139.467},
                    new DataEntry {Id = 218, Period = 218, Value = 1687.449},
                    new DataEntry {Id = 219, Period = 219, Value = 1600.108},
                    new DataEntry {Id = 220, Period = 220, Value = 1269.7},
                    new DataEntry {Id = 221, Period = 221, Value = 1182.436},
                    new DataEntry {Id = 222, Period = 222, Value = 1228.328},
                    new DataEntry {Id = 223, Period = 223, Value = 1388.123},
                    new DataEntry {Id = 224, Period = 224, Value = 1340.499},
                    new DataEntry {Id = 225, Period = 225, Value = 1183.76},
                    new DataEntry {Id = 226, Period = 226, Value = 1163.884},
                    new DataEntry {Id = 227, Period = 227, Value = 1437.454},
                    new DataEntry {Id = 228, Period = 228, Value = 1798.849},
                    new DataEntry {Id = 229, Period = 229, Value = 2045.526},
                    new DataEntry {Id = 230, Period = 230, Value = 1770.089},
                    new DataEntry {Id = 231, Period = 231, Value = 1599.009},
                    new DataEntry {Id = 232, Period = 232, Value = 1356.49},
                    new DataEntry {Id = 233, Period = 233, Value = 1143.744},
                    new DataEntry {Id = 234, Period = 234, Value = 1135.629},
                    new DataEntry {Id = 235, Period = 235, Value = 1325.155},
                    new DataEntry {Id = 236, Period = 236, Value = 1269.507},
                    new DataEntry {Id = 237, Period = 237, Value = 1175.09},
                    new DataEntry {Id = 238, Period = 238, Value = 1177.522},
                    new DataEntry {Id = 239, Period = 239, Value = 1412.72},
                    new DataEntry {Id = 240, Period = 240, Value = 1946.919},
                    new DataEntry {Id = 241, Period = 241, Value = 2100.267},
                    new DataEntry {Id = 242, Period = 242, Value = 1871.852},
                    new DataEntry {Id = 243, Period = 243, Value = 1841.363},
                    new DataEntry {Id = 244, Period = 244, Value = 1381.721},
                    new DataEntry {Id = 245, Period = 245, Value = 1121.875},
                    new DataEntry {Id = 246, Period = 246, Value = 1209.729},
                    new DataEntry {Id = 247, Period = 247, Value = 1466.985},
                    new DataEntry {Id = 248, Period = 248, Value = 1454.854},
                    new DataEntry {Id = 249, Period = 249, Value = 1229.854},
                    new DataEntry {Id = 250, Period = 250, Value = 1188.966},
                    new DataEntry {Id = 251, Period = 251, Value = 1449.399},
                    new DataEntry {Id = 252, Period = 252, Value = 1902.838},
                    new DataEntry {Id = 253, Period = 253, Value = 2344.022},
                    new DataEntry {Id = 254, Period = 254, Value = 1984.091},
                    new DataEntry {Id = 255, Period = 255, Value = 1709.38},
                    new DataEntry {Id = 256, Period = 256, Value = 1308.438},
                    new DataEntry {Id = 257, Period = 257, Value = 1167.867},
                    new DataEntry {Id = 258, Period = 258, Value = 1279.801},
                    new DataEntry {Id = 259, Period = 259, Value = 1440.045},
                    new DataEntry {Id = 260, Period = 260, Value = 1370.424},
                    new DataEntry {Id = 261, Period = 261, Value = 1178.039},
                    new DataEntry {Id = 262, Period = 262, Value = 1169.61},
                    new DataEntry {Id = 263, Period = 263, Value = 1354.226},
                    new DataEntry {Id = 264, Period = 264, Value = 1804.194},
                    new DataEntry {Id = 265, Period = 265, Value = 2089.849},
                    new DataEntry {Id = 266, Period = 266, Value = 1868.378},
                    new DataEntry {Id = 267, Period = 267, Value = 1660.117},
                    new DataEntry {Id = 268, Period = 268, Value = 1337.669},
                    new DataEntry {Id = 269, Period = 269, Value = 1216.161},
                    new DataEntry {Id = 270, Period = 270, Value = 1255.727},
                    new DataEntry {Id = 271, Period = 271, Value = 1493.528},
                    new DataEntry {Id = 272, Period = 272, Value = 1592.922},
                    new DataEntry {Id = 273, Period = 273, Value = 1263.81},
                    new DataEntry {Id = 274, Period = 274, Value = 1194.153},
                    new DataEntry {Id = 275, Period = 275, Value = 1525.984},
                    new DataEntry {Id = 276, Period = 276, Value = 2026.481},
                    new DataEntry {Id = 277, Period = 277, Value = 2362.276},
                    new DataEntry {Id = 278, Period = 278, Value = 2075.92},
                    new DataEntry {Id = 279, Period = 279, Value = 1868.011},
                    new DataEntry {Id = 280, Period = 280, Value = 1457.023},
                    new DataEntry {Id = 281, Period = 281, Value = 1306.948},
                    new DataEntry {Id = 282, Period = 282, Value = 1351.491},
                    new DataEntry {Id = 283, Period = 283, Value = 1497.537},
                    new DataEntry {Id = 284, Period = 284, Value = 1471.334},
                    new DataEntry {Id = 285, Period = 285, Value = 1265.326},
                    new DataEntry {Id = 286, Period = 286, Value = 1251.505},
                    new DataEntry {Id = 287, Period = 287, Value = 1586.807},
                    new DataEntry {Id = 288, Period = 288, Value = 2004.7},
                    new DataEntry {Id = 289, Period = 289, Value = 2299.846},
                    new DataEntry {Id = 290, Period = 290, Value = 1881.633},
                    new DataEntry {Id = 291, Period = 291, Value = 1693.9},
                    new DataEntry {Id = 292, Period = 292, Value = 1388.166},
                    new DataEntry {Id = 293, Period = 293, Value = 1243.279},
                    new DataEntry {Id = 294, Period = 294, Value = 1267.613},
                    new DataEntry {Id = 295, Period = 295, Value = 1555.276},
                    new DataEntry {Id = 296, Period = 296, Value = 1484.2},
                    new DataEntry {Id = 297, Period = 297, Value = 1299.345},
                    new DataEntry {Id = 298, Period = 298, Value = 1297.292},
                    new DataEntry {Id = 299, Period = 299, Value = 1544.144},
                    new DataEntry {Id = 300, Period = 300, Value = 2010.349},
                    new DataEntry {Id = 301, Period = 301, Value = 2133.046},
                    new DataEntry {Id = 302, Period = 302, Value = 1772.061},
                    new DataEntry {Id = 303, Period = 303, Value = 1793.936},
                    new DataEntry {Id = 304, Period = 304, Value = 1361.897},
                    new DataEntry {Id = 305, Period = 305, Value = 1285.546},
                    new DataEntry {Id = 306, Period = 306, Value = 1438.768},
                    new DataEntry {Id = 307, Period = 307, Value = 1678.551},
                    new DataEntry {Id = 308, Period = 308, Value = 1615.373},
                    new DataEntry {Id = 309, Period = 309, Value = 1393.17},
                    new DataEntry {Id = 310, Period = 310, Value = 1282.604},
                    new DataEntry {Id = 311, Period = 311, Value = 1373.219},
                    new DataEntry {Id = 312, Period = 312, Value = 1834.449},
                    new DataEntry {Id = 313, Period = 313, Value = 2370.963},
                    new DataEntry {Id = 314, Period = 314, Value = 1830.929},
                    new DataEntry {Id = 315, Period = 315, Value = 1879.912},
                    new DataEntry {Id = 316, Period = 316, Value = 1450.061},
                    new DataEntry {Id = 317, Period = 317, Value = 1287.919},
                    new DataEntry {Id = 318, Period = 318, Value = 1410.923},
                    new DataEntry {Id = 319, Period = 319, Value = 1710.751},
                    new DataEntry {Id = 320, Period = 320, Value = 1674.714},
                    new DataEntry {Id = 321, Period = 321, Value = 1363.793},
                    new DataEntry {Id = 322, Period = 322, Value = 1281.755},
                    new DataEntry {Id = 323, Period = 323, Value = 1387.046},
                    new DataEntry {Id = 324, Period = 324, Value = 1913.629},
                    new DataEntry {Id = 325, Period = 325, Value = 2349.975},
                    new DataEntry {Id = 326, Period = 326, Value = 2052.109},
                    new DataEntry {Id = 327, Period = 327, Value = 1707.957},
                    new DataEntry {Id = 328, Period = 328, Value = 1428.931},
                    new DataEntry {Id = 329, Period = 329, Value = 1381.162},
                    new DataEntry {Id = 330, Period = 330, Value = 1480.546},
                    new DataEntry {Id = 331, Period = 331, Value = 1626.774},
                    new DataEntry {Id = 332, Period = 332, Value = 1679.114},
                    new DataEntry {Id = 333, Period = 333, Value = 1412.318},
                    new DataEntry {Id = 334, Period = 334, Value = 1327.079},
                    new DataEntry {Id = 335, Period = 335, Value = 1581.541},
                    new DataEntry {Id = 336, Period = 336, Value = 2390.506},
                    new DataEntry {Id = 337, Period = 337, Value = 2568.812},
                    new DataEntry {Id = 338, Period = 338, Value = 2016.103},
                    new DataEntry {Id = 339, Period = 339, Value = 1916.254},
                    new DataEntry {Id = 340, Period = 340, Value = 1466.16},
                    new DataEntry {Id = 341, Period = 341, Value = 1282.278},
                    new DataEntry {Id = 342, Period = 342, Value = 1412.887},
                    new DataEntry {Id = 343, Period = 343, Value = 1642.872},
                    new DataEntry {Id = 344, Period = 344, Value = 1710.942},
                    new DataEntry {Id = 345, Period = 345, Value = 1369.521},
                    new DataEntry {Id = 346, Period = 346, Value = 1331.263},
                    new DataEntry {Id = 347, Period = 347, Value = 1417.543},
                    new DataEntry {Id = 348, Period = 348, Value = 1901.08},
                    new DataEntry {Id = 349, Period = 349, Value = 2298.908},
                    new DataEntry {Id = 350, Period = 350, Value = 1919.854},
                    new DataEntry {Id = 351, Period = 351, Value = 1907.718},
                    new DataEntry {Id = 352, Period = 352, Value = 1509.146},
                    new DataEntry {Id = 353, Period = 353, Value = 1386.968},
                    new DataEntry {Id = 354, Period = 354, Value = 1517.331},
                    new DataEntry {Id = 355, Period = 355, Value = 1768.445},
                    new DataEntry {Id = 356, Period = 356, Value = 1733.749},
                    new DataEntry {Id = 357, Period = 357, Value = 1486.699},
                    new DataEntry {Id = 358, Period = 358, Value = 1424.136},
                    new DataEntry {Id = 359, Period = 359, Value = 1645.814},
                    new DataEntry {Id = 360, Period = 360, Value = 2210.196},
                    new DataEntry {Id = 361, Period = 361, Value = 2568.147},
                    new DataEntry {Id = 362, Period = 362, Value = 2253.393},
                    new DataEntry {Id = 363, Period = 363, Value = 1955.352},
                    new DataEntry {Id = 364, Period = 364, Value = 1488.784},
                    new DataEntry {Id = 365, Period = 365, Value = 1384.703},
                    new DataEntry {Id = 366, Period = 366, Value = 1427.182},
                    new DataEntry {Id = 367, Period = 367, Value = 1709.956},
                    new DataEntry {Id = 368, Period = 368, Value = 1736},
                    new DataEntry {Id = 369, Period = 369, Value = 1462.168},
                    new DataEntry {Id = 370, Period = 370, Value = 1365.646},
                    new DataEntry {Id = 371, Period = 371, Value = 1543.571},
                    new DataEntry {Id = 372, Period = 372, Value = 2209.033},
                    new DataEntry {Id = 373, Period = 373, Value = 2598.736},
                    new DataEntry {Id = 374, Period = 374, Value = 2268.858},
                    new DataEntry {Id = 375, Period = 375, Value = 1841.164},
                    new DataEntry {Id = 376, Period = 376, Value = 1458.264},
                    new DataEntry {Id = 377, Period = 377, Value = 1395.366},
                    new DataEntry {Id = 378, Period = 378, Value = 1517.332},
                    new DataEntry {Id = 379, Period = 379, Value = 1697.931},
                    new DataEntry {Id = 380, Period = 380, Value = 1649.162},
                    new DataEntry {Id = 381, Period = 381, Value = 1464.897},
                    new DataEntry {Id = 382, Period = 382, Value = 1392.647},
                    new DataEntry {Id = 383, Period = 383, Value = 1567.126},
                    new DataEntry {Id = 384, Period = 384, Value = 2239.587},
                    new DataEntry {Id = 385, Period = 385, Value = 2497.62},
                    new DataEntry {Id = 386, Period = 386, Value = 2077.171},
                    new DataEntry {Id = 387, Period = 387, Value = 1998.451},
                    new DataEntry {Id = 388, Period = 388, Value = 1463.477},
                    new DataEntry {Id = 389, Period = 389, Value = 1385.741},
                    new DataEntry {Id = 390, Period = 390, Value = 1594.247},
                    new DataEntry {Id = 391, Period = 391, Value = 1870.208},
                    new DataEntry {Id = 392, Period = 392, Value = 1867.43},
                    new DataEntry {Id = 393, Period = 393, Value = 1567.815},
                    new DataEntry {Id = 394, Period = 394, Value = 1430.78},
                    new DataEntry {Id = 395, Period = 395, Value = 1552.223},
                    new DataEntry {Id = 396, Period = 396, Value = 2322.603},
                    new DataEntry {Id = 397, Period = 397, Value = 2176.334},
                    new DataEntry {Id = 398, Period = 398, Value = 2004.301},
                    new DataEntry {Id = 399, Period = 399, Value = 1926.964},
                    new DataEntry {Id = 400, Period = 400, Value = 1461.001},
                    new DataEntry {Id = 401, Period = 401, Value = 1387.329},
                    new DataEntry {Id = 402, Period = 402, Value = 1567.152},
                    new DataEntry {Id = 403, Period = 403, Value = 1859.834},
                    new DataEntry {Id = 404, Period = 404, Value = 1844.026},
                    new DataEntry {Id = 405, Period = 405, Value = 1429.437},
                    new DataEntry {Id = 406, Period = 406, Value = 1401.001},
                    new DataEntry {Id = 407, Period = 407, Value = 1586.621},
                    new DataEntry {Id = 408, Period = 408, Value = 2053.16},
                    new DataEntry {Id = 409, Period = 409, Value = 2376.733},
                    new DataEntry {Id = 410, Period = 410, Value = 2365.733},
                    new DataEntry {Id = 411, Period = 411, Value = 1929.548},
                    new DataEntry {Id = 412, Period = 412, Value = 1514.155},
                    new DataEntry {Id = 413, Period = 413, Value = 1394.945},
                    new DataEntry {Id = 414, Period = 414, Value = 1540.873},
                    new DataEntry {Id = 415, Period = 415, Value = 1751.298},
                    new DataEntry {Id = 416, Period = 416, Value = 1887.464},
                    new DataEntry {Id = 417, Period = 417, Value = 1566.604},
                    new DataEntry {Id = 418, Period = 418, Value = 1403.973},
                    new DataEntry {Id = 419, Period = 419, Value = 1598.684},
                    new DataEntry {Id = 420, Period = 420, Value = 2238.057},
                    new DataEntry {Id = 421, Period = 421, Value = 2531.286},
                    new DataEntry {Id = 422, Period = 422, Value = 2253.712},
                    new DataEntry {Id = 423, Period = 423, Value = 1983.655},
                    new DataEntry {Id = 424, Period = 424, Value = 1515.336},
                    new DataEntry {Id = 425, Period = 425, Value = 1378.592},
                    new DataEntry {Id = 426, Period = 426, Value = 1617.181},
                    new DataEntry {Id = 427, Period = 427, Value = 1809.514},
                    new DataEntry {Id = 428, Period = 428, Value = 1729.828},
                    new DataEntry {Id = 429, Period = 429, Value = 1438.557},
                    new DataEntry {Id = 430, Period = 430, Value = 1371.645},
                    new DataEntry {Id = 431, Period = 431, Value = 1625.197},
                    new DataEntry {Id = 432, Period = 432, Value = 2343.484},
                    new DataEntry {Id = 433, Period = 433, Value = 2610.009},
                    new DataEntry {Id = 434, Period = 434, Value = 2100.985},
                    new DataEntry {Id = 435, Period = 435, Value = 1896.366},
                    new DataEntry {Id = 436, Period = 436, Value = 1499.858},
                    new DataEntry {Id = 437, Period = 437, Value = 1363.829},
                    new DataEntry {Id = 438, Period = 438, Value = 1520.79},
                    new DataEntry {Id = 439, Period = 439, Value = 1704.206},
                    new DataEntry {Id = 440, Period = 440, Value = 1710.872},
                    new DataEntry {Id = 441, Period = 441, Value = 1415.925},
                    new DataEntry {Id = 442, Period = 442, Value = 1409.128},
                    new DataEntry {Id = 443, Period = 443, Value = 1518.554},
                    new DataEntry {Id = 444, Period = 444, Value = 2314.551}
                };
        }
    }
}