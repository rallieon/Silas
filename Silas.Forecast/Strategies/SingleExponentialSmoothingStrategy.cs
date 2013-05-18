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

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Alpha"))
                throw new ArgumentException("The strategy parameters must include Alpha");

            double currForecast = dataEntries.ElementAt(0).Value;
            double alpha = strategyParameters.Alpha;

            if (dataEntries.Count() < 3 || period < 3)
                currForecast = dataEntries.ElementAt(0).Value;
            else if (dataEntries.Count() > 1 && period <= dataEntries.Count() + 1)
            {
                //start at three since there is no forecast for period 1 
                //and period 2 forecast is set to true datapoint from period 1
                for (int currPeriod = 3; currPeriod <= period; currPeriod++)
                {
                    //sub two since list is 0 index based and we want to go back one period
                    currForecast = (alpha*dataEntries.ElementAt(currPeriod - 2).Value) + ((1 - alpha)*currForecast);
                }
            }
            else
            {
                //the single exponential smoothing model should only be used to predict one period ahead
                //anything after that will just be the same value.
                for (int currPeriod = 3; currPeriod <= dataEntries.Count() + 1; currPeriod++)
                {
                    //sub two since list is 0 index based and we want to go back one period
                    currForecast = (alpha * dataEntries.ElementAt(currPeriod - 2).Value) + ((1 - alpha) * currForecast);
                }
            }

            return new ForecastEntry
            {
                Period = period,
                DataEntry = period > dataEntries.Count() ? dataEntries.Last() : dataEntries.ElementAt(period - 1),
                ForecastValue = currForecast,
                ConfidenceIntervalLow = currForecast,
                ConfidenceIntervalHigh = currForecast,
                IsHoldout = period > dataEntries.Count() * 0.7  //holdout data is always 70 percent
            };
        }
    }
}