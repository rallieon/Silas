using System.Linq;

namespace Silas.Forecast
{
    public class NaieveStrategy : IForecastStrategy
    {
        public int Forecast(int[] data)
        {
            return data.Last();
        }
    }
}