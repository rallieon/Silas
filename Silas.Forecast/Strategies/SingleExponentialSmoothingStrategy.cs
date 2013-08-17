using System;
using System.Collections.Generic;
using System.Linq;
using Silas.Forecast.Models;

namespace Silas.Forecast.Strategies
{
    public class SingleExponentialSmoothingStrategy : IForecastStrategy
    {
        public ForecastEntry Forecast(IEnumerable<DataEntry> dataEntries, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0)
                return null;

            double alpha = strategyParameters.Alpha;
            double value;

            if (dataEntries.Count() < 3 || period < 3)
                value = dataEntries.ElementAt(0).Value;
            else if (dataEntries.Count() > 1 && period <= dataEntries.Count() + 1)
                value = GenerateForecast(3, period, alpha, dataEntries, dataEntries.First().Value);
            else
                value = GenerateForecast(3, dataEntries.Count() + 1, alpha, dataEntries, dataEntries.First().Value);

            return new ForecastEntry
                {
                    Period = period,
                    DataEntry = period > dataEntries.Count() ? dataEntries.Last() : dataEntries.ElementAt(period - 1),
                    ForecastValue = value,
                    ConfidenceIntervalLow = value,
                    ConfidenceIntervalHigh = value,
                    IsHoldout = period > dataEntries.Count()*0.7 //holdout data is always 70 percent
                };
        }

        private double GenerateForecast(int startPeriod, int endPeriod, double alpha, IEnumerable<DataEntry> dataEntries,
                                        double currForecast)
        {
            for (var currPeriod = startPeriod; currPeriod <= endPeriod; currPeriod++)
            {
                //sub two since list is 0 index based and we want to go back one period
                currForecast = (alpha*dataEntries.ElementAt(currPeriod - 2).Value) + ((1 - alpha)*currForecast);
            }

            return currForecast;
        }
    }
}