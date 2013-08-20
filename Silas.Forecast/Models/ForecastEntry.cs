﻿using System;

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
        public double ModelMeanAbsoluteError { get; set; }
        public double ModelPercentError { get; set; }

        public double Error
        {
            get { return DataEntry.Value - ForecastValue; }
        }

        public double AbsError
        {
            get { return Math.Abs(Error); }
        }

        public double SquaredError
        {
            get { return Math.Pow(Error, 2); }
        }
    }
}