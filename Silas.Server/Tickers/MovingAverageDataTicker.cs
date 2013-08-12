using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;
using Silas.Web.Clients;
using Silas.Web.Hubs;

namespace Silas.Web.Tickers
{
    public class MovingAverageDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<MovingAverageDataTicker> _instance =
            new Lazy<MovingAverageDataTicker>(
                () =>
                new MovingAverageDataTicker(
                    GlobalHost.ConnectionManager.GetHubContext<MovingAverageDataHub>().Clients));

        private readonly LiveDataClient _dataClient = new LiveDataClient();

        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly object _forecastLock = new object();
        private readonly Model _model;
        private readonly dynamic _parameters;
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private int _currentPeriod = 1;

        private MovingAverageDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _entries = new ConcurrentDictionary<int, DataEntry>();
            _dataClient.GetData().ToList().ForEach(e => _entries.TryAdd(e.Id, e));
            _timer = new Timer(NextValue, null, Timeout.Infinite, Timeout.Infinite);
            _parameters = new ExpandoObject();
            _parameters.PeriodCount = 4;

            _model = new Model(new MovingAverageStrategy(), _entries.Values, _parameters);
        }

        public static MovingAverageDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                ForecastEntry value = _model.Forecast(_currentPeriod);
                _currentPeriod++;
                SendValue(value);
            }
        }

        public void SendValue(ForecastEntry value)
        {
            Clients.All.sendValue(value);
        }

        public void Start()
        {
            _timer.Change(_updateInterval, _updateInterval);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}