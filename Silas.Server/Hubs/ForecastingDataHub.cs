using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using Repositories.Interfaces;
using Silas.Forecast.Models;
using Silas.Server.Broadcasters;

namespace Silas.Server.Hubs
{
    public class ForecastingDataHub : Hub
    {
        private readonly IForecastingDataBroadcaster _broadcaster;
        private readonly IRepository _repository;

        public ForecastingDataHub(IRepository repository, IForecastingDataBroadcaster broadcaster)
        {
            _repository = repository;
            _broadcaster = broadcaster;
        }


        public DataSet Init(dynamic parameters)
        {
            var set = new DataSet
            {
                CurrentPeriod = 1,
                Entries = new Collection<DataEntry>(),
                Token = Guid.NewGuid().ToString()
            };

            _repository.Create(set);
            Groups.Add(Context.ConnectionId, set.Token);
            _broadcaster.Init(set, _repository, parameters);

            return set;
        }

        public void Register(DataSet set)
        {
            Groups.Add(Context.ConnectionId, set.Token);
        }

        public void Unregister(DataSet set)
        {
            Groups.Remove(Context.ConnectionId, set.Token);
        }

        public void AddEntry(DataSet set, DataEntry entry)
        {
            var setToAddTo = _repository.Get<DataSet>().FirstOrDefault(s => s.Token == set.Token);

            if (setToAddTo == null) return;

            //commit new entry
            entry.DataSetId = setToAddTo.Id;
            setToAddTo.Entries.Add(entry);
            setToAddTo.CurrentPeriod = entry.Period;
            _repository.Update(setToAddTo);

            _broadcaster.SendForecast(set);
        }

        public void ModifyParameters(DataSet set, dynamic parameters)
        {
            _broadcaster.ModifyParameters(set, parameters);
        }

        public void Stop(DataSet set)
        {
            _broadcaster.Stop(set);
        }

        public void Start(DataSet set)
        {
            _broadcaster.Start(set);
        }

        public void Pause(DataSet set)
        {
            _broadcaster.Pause(set);
        }
    }
}