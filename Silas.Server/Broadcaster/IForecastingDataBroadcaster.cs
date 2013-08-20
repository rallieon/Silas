using Repositories.Interfaces;
using Silas.Forecast.Models;

namespace Silas.Server.Broadcasters
{
    public interface IForecastingDataBroadcaster
    {
        void Init(DataSet set, IRepository repository, dynamic parameters);
        void Start(DataSet set);
        void Stop(DataSet set);
        void Pause(DataSet set);
        void SendForecast(DataSet set);
        void ModifyParameters(DataSet set, dynamic parameters);
    }
}