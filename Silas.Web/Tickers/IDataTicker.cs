namespace Silas.Web.Tickers
{
    internal interface IDataTicker
    {
        void NextValue(object state);
        void SendValue(int value);
    }
}