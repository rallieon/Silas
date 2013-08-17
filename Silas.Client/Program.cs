using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Threading;
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
        private static int _currentPeriod;
        private static bool _initialized = false;
        private static dynamic _parameters;

        private static void Main(string[] args)
        {
            _set = new DataSet();
            _parameters = new ExpandoObject();
            _parameters.Strategy = FORECAST_STRATEGY.Naive;

            //Setup initial dataset
            InitializeClients();

            //pause so everything gets init.
            while (!_initialized){}

            SendHistoricalData();

            _proxySender.Invoke("Start", _set).Wait();

            //begin feeding live data constantly
            Console.WriteLine("Begin sending live data.");
            while(true)
                SendLiveData();
        }

        private static void SendHistoricalData()
        {
            foreach (var entry in GetData())
            {
                _currentPeriod = entry.Period;
                _proxySender.Invoke("AddEntry", _set, entry).Wait();
            }

            Console.WriteLine("Finished sending historical data.");
        }

        private static void SendLiveData()
        {
            foreach (var entry in GetData())
            {
                entry.Period = ++_currentPeriod;
                _proxySender.Invoke("AddEntry", _set, entry).Wait();
                Thread.Sleep(1000);
            }
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
            var set = await _proxySender.Invoke("Init", _parameters);

            _set = new DataSet {CurrentPeriod = set.CurrentPeriod, Token = set.Token };
            _retrieverConnection = new HubConnection("http://localhost:8080/");
            _proxyRetriever = _retrieverConnection.CreateHubProxy("ForecastingDataHub");
            _retrieverConnection.Start().Wait();
            _proxyRetriever.Invoke("Register", _set).Wait();
            _proxyRetriever.On<ForecastEntry>("sendValue",
                                              forecast =>
                                              Console.WriteLine("The amount of hits for the period {0} will be {1}",
                                                                forecast.Period, forecast.ForecastValue));

            _initialized = true;
            Console.WriteLine("Finished the initialization of the clients. \nYour token is: " + set.Token);
        }
    }
}