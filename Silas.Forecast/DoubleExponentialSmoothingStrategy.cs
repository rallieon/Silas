#region

using System.Linq;

#endregion

namespace Silas.Forecast
{
    public class DoubleExponentialSmoothingStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period)
        {
            return data.Last();
        }
    }
}