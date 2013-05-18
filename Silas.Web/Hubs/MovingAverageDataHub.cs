using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class MovingAverageDataHub : Hub
    {
        private readonly MovingAverageDataTicker _movingAverageDataTicker;

        public MovingAverageDataHub()
            : this(MovingAverageDataTicker.Instance)
        {
        }

        public MovingAverageDataHub(MovingAverageDataTicker ticker)
        {
            _movingAverageDataTicker = ticker;
        }
    }
}