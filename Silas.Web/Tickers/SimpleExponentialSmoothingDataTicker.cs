using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Silas.Domain;
using Silas.Forecast;
using Silas.Web.Clients;
using Silas.Web.Hubs;

namespace Silas.Web.Tickers
{
    public class SimpleExponentialSmoothingDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<SimpleExponentialSmoothingDataTicker> _instance =
            new Lazy<SimpleExponentialSmoothingDataTicker>(
                () =>
                new SimpleExponentialSmoothingDataTicker(
                    GlobalHost.ConnectionManager.GetHubContext<SimpleExponentialSmoothingDataHub>().Clients));

        private readonly Forecast.Forecast _forecast = new Forecast.Forecast();
        private readonly object _forecastLock = new object();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly LiveDataClient dataClient = new LiveDataClient();

        private SimpleExponentialSmoothingDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _timer = new Timer(NextValue, null, _updateInterval, _updateInterval);
        }

        public static SimpleExponentialSmoothingDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                //run SimpleExponentialSmoothing forecast
                IEnumerable<DataEntry> entries = dataClient.GetData();
                SendValue(_forecast.Execute(ForecastStrategy.SimpleExponentialSmoothing,
                                            entries.Select(e => e.Value).ToArray()));
            }
        }

        public void SendValue(int value)
        {
            Clients.All.sendValue(value);
        }
    }
}