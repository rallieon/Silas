using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silas.Forecast
{
    public enum ForecastStrategy
    {
        Naieve
    }
    public class Forecast
    {
        public int Execute(ForecastStrategy strategy, int[] data)
        {
            int value = 0;
            switch (strategy)
            {
                case ForecastStrategy.Naieve:
                    value = new NaieveStrategy().Forecast(data);
                    break;
            }

            return value;
        }
    }
}
