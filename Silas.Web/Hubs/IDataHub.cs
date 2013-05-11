using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    internal interface IDataHub
    {
        IDataHub(IDataTicker ticker);
    }
}