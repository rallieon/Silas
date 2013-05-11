using System;
using System.Collections.Concurrent;
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
    public class DoubleExponentialSmoothingDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<DoubleExponentialSmoothingDataTicker> _instance =
            new Lazy<DoubleExponentialSmoothingDataTicker>(
                () =>
                new DoubleExponentialSmoothingDataTicker(
                    GlobalHost.ConnectionManager.GetHubContext<DoubleExponentialSmoothingDataHub>().Clients));

        private readonly Forecast.Forecast _forecast = new Forecast.Forecast();
        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly object _forecastLock = new object();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly LiveDataClient _dataClient = new LiveDataClient();
        private int currentPeriod = 1;

        private DoubleExponentialSmoothingDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _entries = new ConcurrentDictionary<int, DataEntry>();
            _dataClient.GetData().ToList().ForEach(e => _entries.TryAdd(e.Id, e));
            _timer = new Timer(NextValue, null, _updateInterval, _updateInterval);
        }

        public static DoubleExponentialSmoothingDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                SendValue(_forecast.Execute(ForecastStrategy.DoubleExponentialSmoothing, _entries.Values.Select(e => e.Value).ToArray(), currentPeriod++));
            }
        }

        public void SendValue(int value)
        {
            Clients.All.sendValue(value);
        }
    }
}