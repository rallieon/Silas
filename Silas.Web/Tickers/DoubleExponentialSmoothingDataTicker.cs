﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Silas.Forecast;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;
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

        private readonly Model _model;
        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly object _forecastLock = new object();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly LiveDataClient _dataClient = new LiveDataClient();
        private int _currentPeriod = 101;
        private readonly dynamic _parameters;

        private DoubleExponentialSmoothingDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _entries = new ConcurrentDictionary<int, DataEntry>();
            _dataClient.GetData(100).ToList().ForEach(e => _entries.TryAdd(e.Id, e));
            _parameters = new ExpandoObject();
            _parameters.Alpha = 0.5;
            _parameters.Beta = 0.5;
            _model = new Model(new DoubleExponentialSmoothingStrategy(), _entries.Values, _parameters);
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
                ForecastEntry value = _model.Forecast(_currentPeriod);
                _currentPeriod++;
                SendValue(value);
            }
        }

        public void SendValue(ForecastEntry value)
        {
            Clients.All.sendValue(value);
        }
    }
}