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

            double value;
            if (dataEntries.Count() == 1 || period == 1)
                value = dataEntries.ElementAt(0).Value;
            else if (dataEntries.Count() > 1 && period <= dataEntries.Count() + 1)
                value = dataEntries.ElementAt(period - 2).Value;
            else
                value = dataEntries.Last().Value;

            return new ForecastEntry
                {
                    Period = period,
                    DataEntry = period > dataEntries.Count() ? dataEntries.Last() : dataEntries.ElementAt(period - 1),
                    ForecastValue = value,
                    ConfidenceIntervalLow = value,
                    ConfidenceIntervalHigh = value,
                    IsHoldout = period > dataEntries.Count() * 0.70  //holdout data is always 70 percent
                };
        }
    }
}