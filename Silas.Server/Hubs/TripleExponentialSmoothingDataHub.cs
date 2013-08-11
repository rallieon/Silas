using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class TripleExponentialSmoothingDataHub : Hub
    {
        private readonly TripleExponentialSmoothingDataTicker _tripleExponentialSmoothingDataTicker;

        public TripleExponentialSmoothingDataHub()
            : this(TripleExponentialSmoothingDataTicker.Instance)
        {
        }

        public TripleExponentialSmoothingDataHub(TripleExponentialSmoothingDataTicker ticker)
        {
            _tripleExponentialSmoothingDataTicker = ticker;
        }

        public void Stop()
        {
            _tripleExponentialSmoothingDataTicker.Stop();
        }

        public void Start()
        {
            _tripleExponentialSmoothingDataTicker.Start();
        }
    }
}