using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
