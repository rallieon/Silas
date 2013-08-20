﻿namespace Silas.Forecast.Models
{
    public class SeasonalIndex
    {
        public int Period { get; set; }
        public double Base { get; set; }
        public double Actual { get; set; }
        public double PeriodSeasonalComponent { get; set; }
    }
}