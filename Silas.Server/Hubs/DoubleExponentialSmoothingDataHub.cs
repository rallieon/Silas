using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class DoubleExponentialSmoothingDataHub : Hub
    {
        private readonly DoubleExponentialSmoothingDataTicker _doubleExponentialSmoothingDataTicker;

        public DoubleExponentialSmoothingDataHub()
            : this(DoubleExponentialSmoothingDataTicker.Instance)
        {
        }

        public DoubleExponentialSmoothingDataHub(DoubleExponentialSmoothingDataTicker ticker)
        {
            _doubleExponentialSmoothingDataTicker = ticker;
        }

        public void Stop()
        {
            _doubleExponentialSmoothingDataTicker.Stop();
        }

        public void Start()
        {
            _doubleExponentialSmoothingDataTicker.Start();
        }
    }
}