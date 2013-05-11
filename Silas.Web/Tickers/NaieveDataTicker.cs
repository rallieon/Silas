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
    public class NaieveDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<NaieveDataTicker> _instance =
            new Lazy<NaieveDataTicker>(
                () => new NaieveDataTicker(GlobalHost.ConnectionManager.GetHubContext<NaieveDataHub>().Clients));

        private readonly Forecast.Forecast _forecast = new Forecast.Forecast();
        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly object _forecastLock = new object();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly LiveDataClient _dataClient = new LiveDataClient();
        private int currentPeriod = 1;

        private NaieveDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _entries = new ConcurrentDictionary<int, DataEntry>();
            _dataClient.GetData(100).ToList().ForEach(e => _entries.TryAdd(e.Id, e));
            _timer = new Timer(NextValue, null, _updateInterval, _updateInterval);
        }

        public static NaieveDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                SendValue(_forecast.Execute(ForecastStrategy.Naieve, _entries.Values.Select(e => e.Value).ToArray(), currentPeriod++));
            }
        }

        public void SendValue(int value)
        {
            Clients.All.sendValue(value);
        }
    }
}