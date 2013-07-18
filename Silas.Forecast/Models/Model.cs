using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silas.Forecast.Strategies;

namespace Silas.Forecast.Models
{
    public class Model
    {
        private readonly IForecastStrategy _forecast;
        private IEnumerable<DataEntry> _originalDataEntries;
        private ICollection<dynamic> _forecastEntries;
        private dynamic _parameters;
        private int _numberOfForecasts = 0;
        private double _absoluteErrorTotal = 0;
        private double _percentAbsoluteErrorTotal = 0;

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
            for (int period = 1; period <= _originalDataEntries.Count(); period++)
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
            _percentAbsoluteErrorTotal += (entry.AbsError) / entry.ForecastValue;
            entry.ModelPercentError = _percentAbsoluteErrorTotal/_numberOfForecasts;

            return entry;
        }
    }
}
