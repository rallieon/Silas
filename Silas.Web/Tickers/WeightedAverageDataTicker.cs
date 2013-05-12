using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
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
    public class WeightedAverageDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<WeightedAverageDataTicker> _instance =
            new Lazy<WeightedAverageDataTicker>(
                () =>
                new WeightedAverageDataTicker(
                    GlobalHost.ConnectionManager.GetHubContext<WeightedAverageDataHub>().Clients));

        private readonly Forecast.Forecast _forecast = new Forecast.Forecast();
        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly object _forecastLock = new object();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly LiveDataClient _dataClient = new LiveDataClient();
        private int currentPeriod = 101;
        private dynamic _parameters;

        private WeightedAverageDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _entries = new ConcurrentDictionary<int, DataEntry>();
            _dataClient.GetData(100).ToList().ForEach(e => _entries.TryAdd(e.Id, e));
            _timer = new Timer(NextValue, null, _updateInterval, _updateInterval);
            _parameters = new ExpandoObject();
            _parameters.NumberOfWeights = 2;
            _parameters.Weights = new[] { 0.5, 0.5 };
        }

        public static WeightedAverageDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                var entry = _dataClient.GetEntryByPeriod(currentPeriod++);
                _entries.TryAdd(entry.Id, entry);
                SendValue(_forecast.Execute(ForecastStrategy.WeightedAverage, _entries.Values.Select(e => e.Value).ToArray(), currentPeriod++, _parameters));
            }
        }

        public void SendValue(int value)
        {
            Clients.All.sendValue(value);
        }
    }
}