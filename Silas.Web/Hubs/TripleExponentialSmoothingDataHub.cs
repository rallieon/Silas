using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class TripleExponentialSmoothingDataHub : Hub
    {
        private readonly TripleExponentialSmoothingDataTicker _doubleExponentialSmoothingDataTicker;

        public TripleExponentialSmoothingDataHub()
            : this(TripleExponentialSmoothingDataTicker.Instance)
        {
        }

        public TripleExponentialSmoothingDataHub(TripleExponentialSmoothingDataTicker ticker)
        {
            _doubleExponentialSmoothingDataTicker = ticker;
        }
    }
}