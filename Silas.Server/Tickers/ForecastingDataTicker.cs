using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Repositories.Interfaces;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;
using Silas.Server.Hubs;

namespace Silas.Server.Tickers
{
    public class ForecastingDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<ForecastingDataTicker> _instance =
            new Lazy<ForecastingDataTicker>(
                () =>
                new ForecastingDataTicker(
                    GlobalHost.ConnectionManager.GetHubContext<ForecastingDataHub>().Clients));

        private ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly object _forecastLock = new object();
        private Model _model;
        private dynamic _parameters;
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private int _currentPeriod = 1;
        private readonly IRepository _repository;

        private ForecastingDataTicker(IHubConnectionContext clients, IRepository repository)
        {
            Clients = clients;
            _repository = repository;
            _timer = new Timer(NextValue, null, Timeout.Infinite, Timeout.Infinite);
        }

        public static ForecastingDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                _parameters = new ExpandoObject();
                _parameters.Alpha = 0.0223259097162289;
                _model = new Model(new SingleExponentialSmoothingStrategy(), _repository.Get<DataEntry>(), _parameters);
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