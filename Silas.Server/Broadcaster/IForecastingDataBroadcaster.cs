using Repositories.Interfaces;
using Silas.Forecast.Models;

namespace Silas.Server.Broadcasters
{
    public interface IForecastingDataBroadcaster
    {
        void SetRepository(IRepository repository);
        void Init(DataSet set, dynamic parameters);
        void Start(DataSet set);
        void Stop(DataSet set);
        void Pause(DataSet set);
        void SendForecast(DataSet set);
        void ModifyParameters(DataSet set, object parameters);
    }
}