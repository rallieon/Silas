using System.Linq;

namespace Silas.Forecast
{
    public class NaieveStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period)
        {
            return data.Last();
        }
    }
}