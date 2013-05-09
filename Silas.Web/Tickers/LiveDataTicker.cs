using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Silas.Domain;
using Silas.Web.Hubs;
using Silas.Forecast;

namespace Silas.Web.Tickers
{
    public class LiveDataTicker
    {// Singleton instance
        private readonly static Lazy<LiveDataTicker> _instance = new Lazy<LiveDataTicker>(() => new LiveDataTicker(GlobalHost.ConnectionManager.GetHubContext<ForecastHub>().Clients));

        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private Forecast.Forecast _forecast = new Forecast.Forecast();
        private readonly object _forecastLock = new object();
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly Timer _timer;
        private volatile bool _updatingEntries = false;

        private LiveDataTicker(IHubConnectionContext clients)
        {
            Clients = clients;

            _entries.Clear();
            
            //initialize entry to first 100 entries
            _entries = new ConcurrentDictionary<int, DataEntry>();
            _entries.TryAdd(1, new DataEntry
                {
                    Id = 1,
                    DateTime = DateTime.Now,
                    Value = 2100
                });

            _timer = new Timer(UpdateDataEntries, null, _updateInterval, _updateInterval);

        }

        public static LiveDataTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext Clients
        {
            get;
            set;
        }

        private void UpdateDataEntries(object state)
        {
            lock (_forecastLock)
            {
                //run Naieve forecast
                BroadcastDataEntry(_forecast.Execute(ForecastStrategy.Naieve, _entries.Values.Select(e => e.Value).ToArray()));
            }
        }

        private void BroadcastDataEntry(int value)
        {
            Clients.All.updateNaieve(value);
        }

    }
}