using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using CsvHelper;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class TrueDataContextInitializer : CreateDatabaseIfNotExists<TrueDataContext>
    {
        protected override void Seed(TrueDataContext context)
        {
            //read from the initial data feed to populate the data feed.
            // AppSettings InitialDataPath
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           ConfigurationSettings.AppSettings["FeedDataPath"]);
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