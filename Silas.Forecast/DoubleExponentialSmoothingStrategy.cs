using System.Linq;

namespace Silas.Forecast
{
    public class DoubleExponentialSmoothingStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period, dynamic strategyParameters)
        {
            return data.Last();
        }
    }
}