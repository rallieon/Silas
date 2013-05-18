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
        private IList<ForecastEntry> _forecastEntries;
        private dynamic _parameters;

        public Model(IForecastStrategy forecast, IEnumerable<DataEntry> initialDataEntries, dynamic strategyParameters)
        {
            _forecast = forecast;
            _originalDataEntries = initialDataEntries;
            _parameters = strategyParameters;

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
            return period <= _originalDataEntries.Count() ? _forecastEntries.ElementAt(period - 1) : _forecast.Forecast(_originalDataEntries, period, _parameters);
        }
    }
}
