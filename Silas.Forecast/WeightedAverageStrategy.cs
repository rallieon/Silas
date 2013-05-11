#region

using System.Linq;

#endregion

namespace Silas.Forecast
{
    public class WeightedAverageStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period)
        {
            return data.Last();
        }
    }
}