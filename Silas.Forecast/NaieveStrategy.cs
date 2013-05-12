using System.Linq;

namespace Silas.Forecast
{
    public class NaieveStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0 || period - 1 > data.Length)
                return 0;

            return data.ElementAt(period - 2);
        }
    }
}