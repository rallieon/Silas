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

namespace Silas.Web.Tickers
{
    public class TrueDataTicker
    {
        // Singleton instance
        private readonly static Lazy<TrueDataTicker> _instance = new Lazy<TrueDataTicker>(() => new TrueDataTicker(GlobalHost.ConnectionManager.GetHubContext<ForecastHub>().Clients));

        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly Timer _timer;

        private TrueDataTicker(IHubConnectionContext clients)
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

        public static TrueDataTicker Instance
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
            
        }

        private void BroadcastDataEntry(int value)
        {
            Clients.All.nextLiveData(value);
        }
    }
}