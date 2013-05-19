using System;
using System.Collections.Generic;
using System.Linq;
using Silas.Forecast.Models;

namespace Silas.Forecast.Strategies
{
    public class DoubleExponentialSmoothingStrategy : IForecastStrategy
    {
        public ForecastEntry Forecast(IEnumerable<DataEntry> dataEntries, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0)
                return null;

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Alpha"))
                throw new ArgumentException("The strategy parameters must include Alpha");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Beta"))
                throw new ArgumentException("The strategy parameters must include Beta");

            double alpha = strategyParameters.Alpha;
            double beta = strategyParameters.Beta;
            double value;

            if (dataEntries.Count() < 3 || period < 3)
                value = dataEntries.ElementAt(0).Value;
            else if (dataEntries.Count() > 1 && period <= dataEntries.Count() + 1)
                value = GenerateForecast(3, period, alpha, beta, dataEntries, dataEntries.First().Value, 0);
            else
                value = GenerateForecast(3, dataEntries.Count() + 1, alpha, beta, dataEntries, dataEntries.First().Value, 0);

            return new ForecastEntry
            {
                Period = period,
                DataEntry = period > dataEntries.Count() ? dataEntries.Last() : dataEntries.ElementAt(period - 1),
                ForecastValue = value,
                ConfidenceIntervalLow = value,
                ConfidenceIntervalHigh = value,
                IsHoldout = period > dataEntries.Count() * 0.7  //holdout data is always 70 percent
            };
        }

        private double GenerateForecast(int startPeriod, int endPeriod, double alpha, double beta, IEnumerable<DataEntry> dataEntries, double currForecast, double currTrend)
        {
            double adjustedForecast = currForecast + currTrend;

            for (int currPeriod = 3; currPeriod <= endPeriod; currPeriod++)
            {
                //need to store temp current forecast since trend calcuation is dependent on it
                double tempCurrForecast = currForecast;

                //sub two since list is 0 index based and we want to go back one period
                currForecast = (alpha * dataEntries.ElementAt(currPeriod - 2).Value) + ((1 - alpha) * (currForecast + currTrend));

                //use old currForecast
                currTrend = (beta * (dataEntries.ElementAt(currPeriod - 2).Value - tempCurrForecast)) + ((1 - beta) * currTrend);

                //calculate adjusted forecast
                adjustedForecast = currForecast + currTrend;
            }

            return adjustedForecast;
        }
    }
}