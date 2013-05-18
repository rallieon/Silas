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

            if(numberOfPeriods > dataEntries.Count())
                throw new ArgumentException("The number of periods can not be greater than the number of entries.");

            double value;
            
            if (dataEntries.Count() == 1 || period == 1)
                value = dataEntries.ElementAt(0).Value;
            else if (dataEntries.Count() > 1 && period <= dataEntries.Count() + 1)
                value = dataEntries.Take(period - 1).Reverse().Take(numberOfPeriods).Reverse().Sum(entry => (entry.Value)) / numberOfPeriods;
            else
                value = dataEntries.Reverse().Take(numberOfPeriods).Reverse().Sum(entry => (entry.Value)) / numberOfPeriods;

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
    }
}