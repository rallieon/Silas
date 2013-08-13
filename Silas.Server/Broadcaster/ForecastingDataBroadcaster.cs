using System;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR.Hubs;
using Repositories.Interfaces;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Server.Broadcasters
{
    public class ForecastingDataBroadcaster : IForecastingDataBroadcaster
    {
        private readonly ConcurrentDictionary<string, ActiveForecast> _runningForecasts;
        private IRepository _repository;

        public ForecastingDataBroadcaster(IHubConnectionContext clients)
        {
            if (clients == null)
            {
                throw new ArgumentNullException("clients");
            }

            Clients = clients;
            _runningForecasts = new ConcurrentDictionary<string, ActiveForecast>();
        }

        private IHubConnectionContext Clients { get; set; }

        public void SetRepository(IRepository repository)
        {
            _repository = repository;
        }

        public void Init(DataSet set, dynamic parameters)
        {
            _runningForecasts.TryAdd(set.Name,
                                     new ActiveForecast
                                         {
                                             Parameters = parameters,
                                             Set = set,
                                             State = FORECAST_STATE.INITIALIZED
                                         });
        }

        public void Start(DataSet set)
        {
            ActiveForecast forecast;
            if (_runningForecasts.TryGetValue(set.Name, out forecast))
            {
                forecast.State = FORECAST_STATE.STARTED;
            }
        }

        public void Stop(DataSet set)
        {
            ActiveForecast forecast;
            if (_runningForecasts.TryGetValue(set.Name, out forecast))
            {
                forecast.State = FORECAST_STATE.STOPPED;
            }
        }

        public void Pause(DataSet set)
        {
            ActiveForecast forecast;
            if (_runningForecasts.TryGetValue(set.Name, out forecast))
            {
                forecast.State = FORECAST_STATE.PAUSED;
            }
        }

        public void SendForecast(DataSet set)
        {
            ActiveForecast forecast;
            if (_runningForecasts.TryGetValue(set.Name, out forecast))
            {
                if (forecast.State == FORECAST_STATE.STARTED)
                {
                    FORECAST_STRATEGY strategyType = forecast.Parameters.Strategy;
                    IForecastStrategy strategy;
                    switch (strategyType)
                    {
                        case FORECAST_STRATEGY.Naive:
                            strategy = new NaieveStrategy();
                            break;
                        case FORECAST_STRATEGY.MovingAverage:
                            strategy = new MovingAverageStrategy();
                            break;
                        case FORECAST_STRATEGY.SingleExp:
                            strategy = new SingleExponentialSmoothingStrategy();
                            break;
                        case FORECAST_STRATEGY.DoubleExp:
                            strategy = new DoubleExponentialSmoothingStrategy();
                            break;
                        case FORECAST_STRATEGY.TripleExp:
                            strategy = new TripleExponentialSmoothingStrategy();
                            break;
                        default:
                            strategy = new NaieveStrategy();
                            break;
                    }
                    var model = new Model(strategy, forecast.Set.Entries, forecast.Parameters);
                    var value = model.Forecast(forecast.Set.CurrentPeriod);
                    Clients.Group(set.Name).sendValue(value);
                    forecast.Set.CurrentPeriod++;
                    _repository.Update(forecast.Set);
                }
            }
        }

        public void ModifyParameters(DataSet set, object parameters)
        {
            ActiveForecast forecast;
            if (_runningForecasts.TryGetValue(set.Name, out forecast))
            {
                forecast.Parameters = parameters;
            }
        }
    }
}