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
    {
        protected override void Seed(LiveDataContext context)
        {
            //only read the first one hundred records from the true data feed.
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           ConfigurationSettings.AppSettings["InitDataPath"]);
            var csv = new CsvReader(new StreamReader(filePath));
            IEnumerable<DataEntry> entryList = csv.GetRecords<DataEntry>();

            foreach (DataEntry entry in entryList)
            {
                context.Entries.Add(entry);
            }

            context.SaveChanges();
        }
    }
}