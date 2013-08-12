using Microsoft.AspNet.SignalR;
using Silas.Server.Tickers;

namespace Silas.Server.Hubs
{
    public class ForecastingDataHub : Hub
    {
        private readonly ForecastingDataTicker _singleExponentialSmoothingDataTicker;

        public ForecastingDataHub()
            : this(ForecastingDataTicker.Instance)
        {
        }

        public ForecastingDataHub(ForecastingDataTicker ticker)
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