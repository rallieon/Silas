namespace Silas.Forecast
{
    public interface IForecastStrategy
    {
        int Forecast(int[] data);
    }
}