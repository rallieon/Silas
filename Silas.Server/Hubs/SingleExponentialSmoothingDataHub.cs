using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class SingleExponentialSmoothingDataHub : Hub
    {
        private readonly SingleExponentialSmoothingDataTicker _singleExponentialSmoothingDataTicker;

        public SingleExponentialSmoothingDataHub()
            : this(SingleExponentialSmoothingDataTicker.Instance)
        {
        }

        public SingleExponentialSmoothingDataHub(SingleExponentialSmoothingDataTicker ticker)
        {
            _singleExponentialSmoothingDataTicker = ticker;
        }

        public void Stop()
        {
            _singleExponentialSmoothingDataTicker.Stop();
        }

        public void Start()
        {
            _singleExponentialSmoothingDataTicker.Start();
        }
    }
}