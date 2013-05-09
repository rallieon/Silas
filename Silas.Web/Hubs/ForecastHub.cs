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
        private readonly LiveDataTicker _forecastTicker;

        public ForecastHub() : this(LiveDataTicker.Instance) { }

        public ForecastHub(LiveDataTicker ticker)
        {
            _forecastTicker = ticker;
        }
    }
}