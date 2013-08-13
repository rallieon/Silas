using System;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Client
{
    internal class Program
    {
        private static HubConnection _connection;
        private static IHubProxy _proxy;
        private static DataSet _set;
        private static dynamic _parameters;

        private static void Main(string[] args)
        {
            InitializeServer();
            InitializeForecast();
        }

        private static void InitializeForecast()
        {
            _set = new DataSet {CurrentPeriod = 1, Entries = null, Name = "SalesForecast", Id = 1};
            _parameters = new ExpandoObject();
            _parameters.Strategy = FORECAST_STRATEGY.SingleExp;
            _parameters.Alpha = 0.0223259097162289;
            _proxy.Invoke("Init", _set, _parameters).Wait();
        }

        private async static void InitializeServer()
        {
            _connection = new HubConnection("http://localhost:8080/");
            _proxy = _connection.CreateHubProxy("ForecastingDataHub");
            _connection.Start().Wait();
        }
    }
}