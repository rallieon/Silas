﻿using System.Collections.Generic;
using Silas.Forecast.Models;

namespace Silas.Web.Clients
{
    internal interface IDataClient
    {
        IEnumerable<DataEntry> GetData(int numberOfRecords);
        DataEntry GetEntryByPeriod(int period);
    }
}