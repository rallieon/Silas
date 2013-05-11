﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Silas.Domain;
using Silas.Web.Clients;
using Silas.Web.Hubs;

namespace Silas.Web.Tickers
{
    public class TrueDataTicker : IDataTicker
    {
        // Singleton instance
        private static readonly Lazy<TrueDataTicker> _instance =
            new Lazy<TrueDataTicker>(
                () => new TrueDataTicker(GlobalHost.ConnectionManager.GetHubContext<TrueDataHub>().Clients));

        private readonly LiveDataClient _dataClient = new LiveDataClient();
        private readonly object _forecastLock = new object();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private int currentPeriod = 1;

        private TrueDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;
            _timer = new Timer(NextValue, null, _updateInterval, _updateInterval);
        }

        public static TrueDataTicker Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public void NextValue(object state)
        {
            lock (_forecastLock)
            {
                SendValue(_dataClient.GetLatestEntry().Value);
            }
        }

        public void SendValue(int value)
        {
            Clients.All.sendValue(value);
        }
    }
}