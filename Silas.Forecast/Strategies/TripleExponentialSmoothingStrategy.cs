using System;
using System.Collections.Generic;
using System.Linq;
using Silas.Forecast.Models;

namespace Silas.Forecast.Strategies
{
    public class TripleExponentialSmoothingStrategy : IForecastStrategy
    {
        private double _alpha;
        private double _beta;
        private double _gamma;
        private int _periodsPerSeason;
        private int _seasonsForRegression;
        private double _a;
        private double _b;

        public ForecastEntry Forecast(IEnumerable<DataEntry> dataEntries, int period, dynamic strategyParameters)
        {
            if (period - 1 < 0)
                return null;

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Alpha"))
                throw new ArgumentException("The strategy parameters must include Alpha");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Beta"))
                throw new ArgumentException("The strategy parameters must include Beta");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("Gamma"))
                throw new ArgumentException("The strategy parameters must include Gamma");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("PeriodsPerSeason"))
                throw new ArgumentException("The strategy parameters must include PeriodsPerSeason");

            if (!((IDictionary<String, object>)strategyParameters).ContainsKey("SeasonsForRegression"))
                throw new ArgumentException("The strategy parameters must include SeasonsForRegression");

            _alpha = strategyParameters.Alpha;
            _beta = strategyParameters.Beta;
            _gamma = strategyParameters.Gamma;
            _periodsPerSeason = strategyParameters.PeriodsPerSeason;
            _seasonsForRegression = strategyParameters.SeasonsForRegression;

            if(_periodsPerSeason * _seasonsForRegression > dataEntries.Count())
                throw new ArgumentException("You need at least more seasons of data than SeasonsForRegression to build a data model.");

            if (period > dataEntries.Count() + _periodsPerSeason)
                throw new ArgumentException("You cannot forecast more than one future ahead.");
            
            //first setup the seasonal indices
            IList<SeasonalIndex> seasonalIndices = BuildSeasonalIndices(dataEntries);
            double value;

            if (dataEntries.Count() < 3 || period < 3)
                value = dataEntries.ElementAt(0).Value;
            else
                value = GenerateForecast(1, period, dataEntries, seasonalIndices);

            return new ForecastEntry
            {
                Period = period,
                DataEntry = period > dataEntries.Count() ? dataEntries.Last() : dataEntries.ElementAt(period - 1),
                ForecastValue = value,
                ConfidenceIntervalLow = value,
                ConfidenceIntervalHigh = value,
                IsHoldout = period > dataEntries.Count() * 0.7  //holdout data is always 70 percent
            };
        }

        /// <summary>
        /// Start period must be the true start of forecasting, not the seasonal indices avareage step.
        /// </summary>
        /// <returns></returns>
        private double GenerateForecast(int startPeriod, int endPeriod, IEnumerable<DataEntry> dataEntries, IList<SeasonalIndex> seasonalIndices)
        {
            IList<TripleExponentialEntry> currSeason = new List<TripleExponentialEntry>();
            int currPeriod;
            bool calculateForecastSeason = false;
            int periodToForecast = endPeriod;

            //determine true endPeriod
            if (endPeriod > dataEntries.Count())
            {
                endPeriod = dataEntries.Count();
                calculateForecastSeason = true;
            }
            //First Build the first seasons indices
            currSeason = BuildFirstSeason(seasonalIndices);

            //Second build the second season
            currSeason = BuildSecondSeason(seasonalIndices);
            
            //Season three and on is the same
            for (currPeriod = startPeriod + (_periodsPerSeason * 2); currPeriod < endPeriod; currPeriod += _periodsPerSeason)
            {
                currSeason = BuildNextSeason(currSeason, dataEntries, currPeriod);
            }

            //handle forecasted last season special (only one season ahead can be calculated)
            if (calculateForecastSeason)
                currSeason = BuildLastSeason(currSeason, dataEntries, currPeriod);

            return currSeason.ElementAt((periodToForecast - 1) % 12).Forecast;
        }

        private IList<TripleExponentialEntry> BuildLastSeason(IList<TripleExponentialEntry> currSeason, IEnumerable<DataEntry> dataEntries, int currPeriod)
        {
            //building the last season is similar to NextSeason, but uses the same Ft and Tt once calculated for the first entry.
            double currentFt = currSeason.Last().Ft;
            double currentTt = currSeason.Last().Tt;
            double currentSt = currSeason.Last().St, lastSt = currSeason.Last().St;
            double lastActual = dataEntries.Last().Value;
            IList<TripleExponentialEntry> newSeason = new List<TripleExponentialEntry>();

            currentFt = (_alpha*(lastActual/lastSt)) + ((1 - _alpha)*(currentTt + currentFt));
            currentTt = (_beta * (currentFt - currSeason.Last().Ft)) + ((1 - _beta) * currentTt);
            
            for (int currSeasonIndex = 1; currSeasonIndex <= _periodsPerSeason; currSeasonIndex++)
            {
                double lastPeriodActual = dataEntries.ElementAt((currPeriod + currSeasonIndex) - _periodsPerSeason - 2).Value;
                double lastPeriodFt = currSeason.ElementAt(currSeasonIndex - 1).Ft;
                double lastPeriodSt = currSeason.ElementAt(currSeasonIndex - 1).St;

                currentSt = (_gamma * (lastPeriodActual / lastPeriodFt)) + ((1 - _gamma) * lastPeriodSt);

                newSeason.Add(new TripleExponentialEntry
                {
                    Ft = currentFt,
                    Tt = currentTt,
                    St = currentSt,
                    Forecast = (currentFt + (currentTt * currSeasonIndex)) * currentSt
                });
            }

            return newSeason;
        }

        private IList<TripleExponentialEntry> BuildNextSeason(IList<TripleExponentialEntry> currSeason, IEnumerable<DataEntry> dataEntries, int currPeriod)
        {
            double currentFt = currSeason.Last().Ft, lastFt = currSeason.Last().Ft;
            double currentTt = currSeason.Last().Tt, lastTt = currSeason.Last().Tt;
            double currentSt = currSeason.Last().St, lastSt = currSeason.Last().St;
            double lastActual = dataEntries.ElementAt(currPeriod - 2).Value;
            IList<TripleExponentialEntry> newSeason = new List<TripleExponentialEntry>();

            for (int currSeasonIndex = 1; currSeasonIndex <= _periodsPerSeason; currSeasonIndex++)
            {
                currentFt = (_alpha*(lastActual/lastSt)) + ((1 - _alpha)*(lastTt+lastFt));
                currentTt = (_beta*(currentFt - lastFt)) + ((1 - _beta)*lastTt);

                double lastPeriodActual = dataEntries.ElementAt( (currPeriod + currSeasonIndex) - _periodsPerSeason - 2).Value;
                double lastPeriodFt = currSeason.ElementAt(currSeasonIndex - 1).Ft;
                double lastPeriodSt = currSeason.ElementAt(currSeasonIndex - 1).St;

                currentSt = (_gamma*(lastPeriodActual/lastPeriodFt)) + ((1 - _gamma)*lastPeriodSt);

                newSeason.Add(new TripleExponentialEntry
                    {
                        Ft = currentFt,
                        Tt = currentTt,
                        St = currentSt,
                        Forecast = (currentFt + currentTt) * currentSt
                    });

                //reset temp vars
                lastFt = currentFt;
                lastTt = currentTt;
                lastSt = currentSt;
                if (currPeriod + currSeasonIndex - 2 >= dataEntries.Count())
                    lastActual = dataEntries.Last().Value;
                else
                    lastActual = dataEntries.ElementAt(currPeriod + currSeasonIndex - 2).Value;
            }

            return newSeason;
        }

        private IList<TripleExponentialEntry> BuildSecondSeason(IList<SeasonalIndex> seasonalIndices)
        {
            //the second season is built by averaging the indices for St
            //Ft is set as the Base value from seasonal index
            //Tt is the slope of the trend line
            IList<TripleExponentialEntry> season = new List<TripleExponentialEntry>();

            for (int currSeasonIndex = 1; currSeasonIndex <= _periodsPerSeason; currSeasonIndex++)
            {
                double currentFt = seasonalIndices.First(e => e.Period == (currSeasonIndex + _periodsPerSeason)).Base;
                double currentTt = _a;
                double currentSt =
                    seasonalIndices.Where(
                        i =>
                        ((i.Period%_periodsPerSeason) == 0 ? _periodsPerSeason : (i.Period%_periodsPerSeason)) ==
                        currSeasonIndex).Average(i => i.PeriodSeasonalComponent);

                season.Add(new TripleExponentialEntry
                    {
                        Ft = currentFt,
                        Tt = currentTt,
                        St = currentSt,
                        Forecast = (currentFt + currentTt) * currentSt
                    });
            }

            return season;
        }

        private IList<TripleExponentialEntry> BuildFirstSeason(IList<SeasonalIndex> seasonalIndices)
        {
            return seasonalIndices.Take(_periodsPerSeason).Select(i => new TripleExponentialEntry
                {
                    Ft = 0,
                    Tt = 0,
                    St = i.PeriodSeasonalComponent,
                    Forecast = 0
                }).ToList();
        }

        private IList<SeasonalIndex> BuildSeasonalIndices(IEnumerable<DataEntry> dataEntries)
        {
            Regression(dataEntries.Select(e => e.Period).Take(_periodsPerSeason * _seasonsForRegression).ToArray(),
                       dataEntries.Select(e => e.Value).Take(_periodsPerSeason * _seasonsForRegression).ToArray(), out _a, out _b);

            //build index for each value
            IList<SeasonalIndex> indices = new List<SeasonalIndex>();
            foreach (DataEntry entry in dataEntries.Take(_periodsPerSeason*(_seasonsForRegression - 1)))
            {
                indices.Add(new SeasonalIndex
                    {
                        Period = entry.Period,
                        Actual = entry.Value,
                        Base = (_a*entry.Period + _b),
                        PeriodSeasonalComponent = entry.Value / (_a*entry.Period + _b)
                    });
            }

            return indices;
        }

        private void Regression(int[] xValues, double[] yValues, out double a, out double b)
        {
            double xAvg = xValues.Average();
            double yAvg = yValues.Average();

            double v1 = 0;
            double v2 = 0;

            for (int x = 0; x < yValues.Length; x++)
            {
                v1 += (x - xAvg) * (yValues[x] - yAvg);
                v2 += Math.Pow(x - xAvg, 2);
            }

            a = v1 / v2;
            b = yAvg - a * xAvg;
        }

        private class TripleExponentialEntry
        {
            public double Ft { get; set; }
            public double Tt { get; set; }
            public double St { get; set; }
            public double Forecast { get; set; }
        }
    }
}