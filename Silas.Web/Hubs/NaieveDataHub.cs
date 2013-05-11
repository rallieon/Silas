using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class NaieveDataHub : Hub, IDataHub
    {
        private readonly NaieveDataTicker _forecastTicker;

        public NaieveDataHub() : this(NaieveDataTicker.Instance)
        {
        }

        public NaieveDataHub(NaieveDataTicker ticker)
        {
            _forecastTicker = ticker;
        }
    }
}