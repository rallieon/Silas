using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silas.Forecast.Models
{
    public enum FORECAST_STATE
    {
        INITIALIZED,
        STARTED,
        STOPPED,
        PAUSED
    }
    public class ActiveForecast
    {
        public DataSet Set { get; set; }
        public dynamic Parameters { get; set; }
        public FORECAST_STATE State { get; set; }
    }
}
