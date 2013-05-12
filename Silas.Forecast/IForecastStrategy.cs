namespace Silas.Forecast
{
    public interface IForecastStrategy
    {
        int Forecast(int[] data, int period, dynamic strategyParameters);
    }
}