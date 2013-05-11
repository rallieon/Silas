using System.Linq;

namespace Silas.Forecast
{
    public class NaieveStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period)
        {
            return period - 1 >= 0 && period - 1 < data.Length ? data.ElementAt(period - 1) : 0;
        }
    }
}