using System.Collections.Generic;
using Silas.Forecast.Models;

namespace Silas.Forecast.Strategies
{
    public enum FORECAST_STRATEGY
    {
        Naive,
        MovingAverage,
        SingleExp,
        DoubleExp,
        TripleExp
    }
    public interface IForecastStrategy
    {
        ForecastEntry Forecast(IEnumerable<DataEntry> dataEntries, int period, dynamic strategyParameters);
    }
}