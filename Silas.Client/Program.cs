using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using CsvHelper;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Silas.Forecast.Models;
using Silas.Forecast.Strategies;

namespace Silas.Client
{
    internal class Program
    {
        private static HubConnection _senderConnection;
        private static IHubProxy _proxySender;
        private static HubConnection _retrieverConnection;
        private static IHubProxy _proxyRetriever;
        private static DataSet _set;
        private static dynamic _parameters;

        private static void Main(string[] args)
        {
            _set = new DataSet {CurrentPeriod = 1, Entries = null, Name = "SalesForecast", Id = 1};
            _parameters = new ExpandoObject();
            _parameters.Strategy = FORECAST_STRATEGY.SingleExp;
            _parameters.Alpha = 0.0223259097162289;

            //Setup initial dataset
            InitializeClients();
            SendHistoricalData();

            _proxySender.Invoke("Start", _set).Wait();

            //begin feeding live data.
            SendLiveData();
        }

        private static void SendHistoricalData()
        {
            foreach (var entry in GetData())
            {
                _proxySender.Invoke("AddEntry", entry).Wait();
            }

            Console.WriteLine("Finished sending historical data.");
        }

        private static void SendLiveData()
        {
            Console.WriteLine("Begin sending live data.");
        }

        private static IEnumerable<DataEntry> GetData()
        {
            //run an api call to the web api layer every second to create stream of hits
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           ConfigurationSettings.AppSettings["FeedDataPath"]);
            var csv = new CsvReader(new StreamReader(filePath));
            csv.Configuration.ClassMapping<DataEntryMap, DataEntry>();

            var entryList = csv.GetRecords<DataEntry>();

            return entryList;
        }

        private static async void InitializeClients()
        {
            _senderConnection = new HubConnection("http://localhost:8080/");
            _proxySender = _senderConnection.CreateHubProxy("ForecastingDataHub");
            _senderConnection.Start().Wait();
            _proxySender.Invoke("Init", _set, _parameters).Wait();


            _retrieverConnection = new HubConnection("http://localhost:8080/");
            _proxyRetriever = _retrieverConnection.CreateHubProxy("ForecastingDataHub");
            _retrieverConnection.Start().Wait();
            _proxyRetriever.Invoke("Register", _set).Wait();
            _proxyRetriever.On<ForecastEntry>("sendValue",
                                              forecast =>
                                              Console.WriteLine("The amount of hits for the period {0} will be {1}",
                                                                forecast.Period, forecast.ForecastValue));

            Console.WriteLine("Finished the initialization of the clients.");
        }
    }
}