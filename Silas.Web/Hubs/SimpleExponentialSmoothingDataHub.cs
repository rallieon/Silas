using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class SimpleExponentialSmoothingDataHub : Hub
    {
        private readonly SimpleExponentialSmoothingDataTicker _simpleExponentialSmoothingDataTicker;

        public SimpleExponentialSmoothingDataHub()
            : this(SimpleExponentialSmoothingDataTicker.Instance)
        {
        }

        public SimpleExponentialSmoothingDataHub(SimpleExponentialSmoothingDataTicker ticker)
        {
            _simpleExponentialSmoothingDataTicker = ticker;
        }
    }
}