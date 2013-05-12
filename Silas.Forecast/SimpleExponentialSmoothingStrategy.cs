using System.Linq;

namespace Silas.Forecast
{
    public class SimpleExponentialSmoothingStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period, dynamic strategyParameters)
        {
            return data.Last();
        }
    }
}