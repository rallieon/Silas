using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class SingleExponentialSmoothingDataHub : Hub
    {
        private readonly SingleExponentialSmoothingDataTicker _simpleExponentialSmoothingDataTicker;

        public SingleExponentialSmoothingDataHub()
            : this(SingleExponentialSmoothingDataTicker.Instance)
        {
        }

        public SingleExponentialSmoothingDataHub(SingleExponentialSmoothingDataTicker ticker)
        {
            _simpleExponentialSmoothingDataTicker = ticker;
        }
    }
}