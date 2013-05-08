using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Silas.Domain;

namespace Silas.Web.Hubs
{
    public class ForecastHub : Hub
    {
        private readonly ForecastTicker _forecastTicker;

        public ForecastHub() : this(ForecastTicker.Instance) { }

        public ForecastHub(ForecastTicker stockTicker)
        {
            _forecastTicker = stockTicker;
        }

        public IEnumerable<DataEntry> GetAllEntries()
        {
            return _forecastTicker.GetAllEntries();
        }

        public void Send(int value)
        {
            Clients.All.sendValue(value);
        }
    }
}