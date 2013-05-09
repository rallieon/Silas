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

namespace Silas.Web
{
    public class ForecastTicker
    {// Singleton instance
        private readonly static Lazy<ForecastTicker> _instance = new Lazy<ForecastTicker>(() => new ForecastTicker(GlobalHost.ConnectionManager.GetHubContext<ForecastHub>().Clients));

        private readonly ConcurrentDictionary<int, DataEntry> _entries = new ConcurrentDictionary<int, DataEntry>();

        private readonly object _forecastLock = new object();
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1000);
        private readonly Timer _timer;
        private volatile bool _updatingEntries = false;

        private ForecastTicker(IHubConnectionContext clients)
        {
            Clients = clients;

            _entries.Clear();
            var entries = new List<DataEntry>
            {
                new DataEntry{ DateTime = DateTime.Now, Value = 2000, Id=1}
            };
            entries.ForEach(entry => _entries.TryAdd(entry.Id, entry));

            _timer = new Timer(UpdateDataEntries, null, _updateInterval, _updateInterval);

        }

        public static ForecastTicker Instance
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

        public IEnumerable<DataEntry> GetAllEntries()
        {
            return _entries.Values;
        }

        private void UpdateDataEntries(object state)
        {
            lock (_forecastLock)
            {
                if (!_updatingEntries)
                {
                    _updatingEntries = true;

                    foreach (var entry in _entries.Values)
                    {
                        if (TryUpdateDataEntry(entry))
                        {
                            BroadcastDataEntry(entry);
                        }
                    }

                    _updatingEntries = false;
                }
            }
        }

        private bool TryUpdateDataEntry(DataEntry dataEntry)
        {
            dataEntry.Value = dataEntry.Value + 100;
            return true;
        }

        private void BroadcastDataEntry(DataEntry entry)
        {
            Clients.All.updateDataEntry(entry);
        }

    }
}