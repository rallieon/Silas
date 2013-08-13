using System.Linq;
using Microsoft.AspNet.SignalR;
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


        public void Init(DataSet set, dynamic parameters)
        {
            set.CurrentPeriod = 1;
            _repository.Create(set);
            Groups.Add(Context.ConnectionId, set.Name);
            _broadcaster.Init(set, parameters);
        }

        public void Register(DataSet set)
        {
            Groups.Add(Context.ConnectionId, set.Name);
        }

        public void AddEntry(DataSet set, DataEntry entry)
        {
            var setToAddTo = _repository.Get<DataSet>().FirstOrDefault(s => s.Id == set.Id);

            if (setToAddTo == null) return;
            setToAddTo.Entries.Add(entry);
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