#region

using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

#endregion

namespace Silas.Web.Hubs
{
    public class WeightedAverageDataHub : Hub
    {
        private readonly WeightedAverageDataTicker _weightedAverageDataTicker;

        public WeightedAverageDataHub()
            : this(WeightedAverageDataTicker.Instance)
        {
        }

        public WeightedAverageDataHub(WeightedAverageDataTicker ticker)
        {
            _weightedAverageDataTicker = ticker;
        }
    }
}