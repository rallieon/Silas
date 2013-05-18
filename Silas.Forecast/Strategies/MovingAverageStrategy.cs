using System;
using System.Collections.Generic;
using System.Linq;
using Silas.Forecast.Models;

namespace Silas.Forecast.Strategies
{
    public class MovingAverageStrategy : IForecastStrategy
    {
        public ForecastEntry Forecast(IEnumerable<DataEntry> dataEntries, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0)
                return null;

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("PeriodCount"))
                throw new ArgumentException("The strategy parameters must include PeriodCount");

            int numberOfPeriods = strategyParameters.PeriodCount;
            double total = dataEntries.Take(period - 1).Reverse().Take(numberOfPeriods).Reverse().Sum(entry => (entry.Value));

            return new ForecastEntry
                {
                    Period = period,
                    DataEntry = period > dataEntries.Count() ? dataEntries.Last() : dataEntries.ElementAt(period - 1),
                    ForecastValue = total / numberOfPeriods,
                    ConfidenceIntervalLow = total / numberOfPeriods,
                    ConfidenceIntervalHigh = total / numberOfPeriods,
                    IsHoldout = period > dataEntries.Count() * 0.7  //holdout data is always 70 percent
                };
        }
    }
}