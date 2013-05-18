using Silas.Forecast.Models;

namespace Silas.Web.Tickers
{
    internal interface IDataTicker
    {
        void NextValue(object state);
        void SendValue(ForecastEntry value);
    }
}