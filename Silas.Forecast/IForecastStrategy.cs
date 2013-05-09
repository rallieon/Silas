using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silas.Forecast
{
    public interface IForecastStrategy
    {
        int Forecast(int[] data);
    }
}
