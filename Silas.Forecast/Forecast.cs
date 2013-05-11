﻿namespace Silas.Forecast
{
    public enum ForecastStrategy
    {
        Naieve,
        WeightedAverage,
        SimpleExponentialSmoothing,
        DoubleExponentialSmoothing
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
                case ForecastStrategy.WeightedAverage:
                    value = new WeightedAverageStrategy().Forecast(data);
                    break;
                case ForecastStrategy.SimpleExponentialSmoothing:
                    value = new SimpleExponentialSmoothingStrategy().Forecast(data);
                    break;
                case ForecastStrategy.DoubleExponentialSmoothing:
                    value = new DoubleExponentialSmoothingStrategy().Forecast(data);
                    break;
            }

            return value;
        }
    }
}