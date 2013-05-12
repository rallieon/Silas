using System;
using System.Collections.Generic;
using System.Linq;

namespace Silas.Forecast
{
    public class WeightedAverageStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0 || period - 1 > data.Length)
                return 0;

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("NumberOfWeights"))
                throw new ArgumentException("The strategy parameters must include NumberOfWeights");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Weights"))
                throw new ArgumentException("The strategy parameters must include Weights");

            int numberOfWeights = strategyParameters.NumberOfWeights;
            double[] weights = strategyParameters.Weights;

            int average = 0;
            int counter = 0;

            foreach (int num in data.Take(period - 1).Reverse().Take(numberOfWeights).Reverse())
            {
                average += (int) (num*weights[counter++]);
            }

            return average;
        }
    }
}