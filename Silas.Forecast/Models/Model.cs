using System;
using System.Collections.Generic;
using System.Linq;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Models
{
    public class Model
    {
        private readonly IForecastStrategy _forecast;
        private readonly ICollection<dynamic> _forecastEntries;
        private readonly IEnumerable<DataEntry> _originalDataEntries;
        private readonly dynamic _parameters;
        private double _absoluteErrorTotal;
        private int _numberOfForecasts;
        private double _percentageTotal;

        public Model(IForecastStrategy forecast, IEnumerable<DataEntry> initialDataEntries, dynamic strategyParameters)
        {
            _forecast = forecast;
            _originalDataEntries = initialDataEntries;
            _parameters = strategyParameters;
            _forecastEntries = new List<dynamic>();
            BuildModel();
        }

        private void BuildModel()
        {
            //go through and build forecast entry for each period in the model.
            for (var period = 1; period <= _originalDataEntries.Count(); period++)
            {
                _forecastEntries.Add(_forecast.Forecast(_originalDataEntries, period, _parameters));
            }
        }

        public ForecastEntry Forecast(int period)
        {
            ForecastEntry entry = period <= _originalDataEntries.Count()
                                      ? _forecastEntries.ElementAt(period - 1)
                                      : _forecast.Forecast(_originalDataEntries, period, _parameters);

            _numberOfForecasts++;
            _absoluteErrorTotal += entry.AbsError;

            entry.ModelMeanAbsoluteError = _absoluteErrorTotal/_numberOfForecasts;
            _percentageTotal += Math.Abs(entry.AbsError/entry.DataEntry.Value);
            entry.ModelPercentError = _percentageTotal/_numberOfForecasts;

            return entry;
        }
    }
}