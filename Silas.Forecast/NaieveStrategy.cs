using System.Linq;

namespace Silas.Forecast
{
    public class NaieveStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period)
        {
            //check minus 2 due to 0 index
            return period - 2 >= 0 && period - 2 < data.Length ? data.ElementAt(period - 2) : 0;
        }
    }
}