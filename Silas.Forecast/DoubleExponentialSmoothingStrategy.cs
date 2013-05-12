using System;
using System.Collections.Generic;
using System.Linq;

namespace Silas.Forecast
{
    public class DoubleExponentialSmoothingStrategy : IForecastStrategy
    {
        public int Forecast(int[] data, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0 || period - 1 > data.Length)
                return 0;

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Alpha"))
                throw new ArgumentException("The strategy parameters must include Alpha");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Beta"))
                throw new ArgumentException("The strategy parameters must include Beta");

            //initial forecast is set to to true data point
            double currForecast = data[0];
            double currTrend = 0;
            double adjustedForecast = currForecast + currTrend;
            double alpha = strategyParameters.Alpha;
            double beta = strategyParameters.Beta;

            //start at three since there is no forecast for period 1 
            //and period 2 forecast is set to true datapoint from period 1
            for (int currPeriod = 3; currPeriod <= period; currPeriod++)
            {
                //need to store temp current forecast since trend calcuation is dependent on it
                double tempCurrForecast = currForecast;                

                //sub two since list is 0 index based and we want to go back one period
                currForecast = (alpha * data[currPeriod - 2]) + ((1 - alpha) * (currForecast + currTrend));

                //use old currForecast
                currTrend = (beta * (data[currPeriod - 2] - tempCurrForecast)) + ((1 - beta) * currTrend);

                //calculate adjusted forecast
                adjustedForecast = currForecast + currTrend;
            }

            return (int)adjustedForecast;
        }
    }
}