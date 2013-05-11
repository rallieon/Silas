using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class TrueDataHub : Hub
    {
        private readonly TrueDataTicker _trueDataTicker;

        public TrueDataHub()
            : this(TrueDataTicker.Instance)
        {
        }

        public TrueDataHub(TrueDataTicker ticker)
        {
            _trueDataTicker = ticker;
        }
    }
}