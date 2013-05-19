using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class NaieveDataHub : Hub
    {
        private readonly NaieveDataTicker _naieveDataTicker;

        public NaieveDataHub() : this(NaieveDataTicker.Instance)
        {
        }

        public NaieveDataHub(NaieveDataTicker ticker)
        {
            _naieveDataTicker = ticker;
        }

        public void Stop()
        {
            _naieveDataTicker.Stop();
        }

        public void Start()
        {
            _naieveDataTicker.Start();
        }
    }
}