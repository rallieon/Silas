using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silas.Forecast.Models
{
    public class ForecastEntry
    {
        public DataEntry DataEntry { get; set; }
        public double ForecastValue { get; set; }
        public double ConfidenceIntervalLow { get; set; }
        public double ConfidenceIntervalHigh { get; set; }
        public int Period { get; set; }
        public bool IsHoldout { get; set; }

        public double Error
        {
            get { return ForecastValue - DataEntry.Value; }
        }

        public double AbsError
        {
            get { return Math.Abs(Error); }
        }
    }
}
