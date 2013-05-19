using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using CsvHelper;
using Silas.Forecast.Models;

namespace Silas.Server.DB
{
    public class LiveDataContextInitializer : DropCreateDatabaseAlways<LiveDataContext>
    { }
}