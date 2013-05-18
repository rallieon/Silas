using System.Collections.Generic;
using System.Linq;
using Silas.Forecast.Models;

namespace Silas.Forecast.Strategies
{
    public class NaieveStrategy : IForecastStrategy
    {
        public ForecastEntry Forecast(IEnumerable<DataEntry> dataEntries, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0)
                return null;

            double value = dataEntries.Count() > 1 ? dataEntries.ElementAt(period - 2).Value : dataEntries.ElementAt(0).Value;

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