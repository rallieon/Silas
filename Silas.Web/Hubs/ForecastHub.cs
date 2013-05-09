using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Silas.Domain;
using Silas.Web.Tickers;

namespace Silas.Web.Hubs
{
    public class ForecastHub : Hub
    {
        private readonly NaieveTicker _forecastTicker;

        public ForecastHub() : this(NaieveTicker.Instance) { }

        public ForecastHub(NaieveTicker ticker)
        {
            _forecastTicker = ticker;
        }
    }
}