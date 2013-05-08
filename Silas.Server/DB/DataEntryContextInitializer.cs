using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using CsvHelper;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class DataEntryContextInitializer : CreateDatabaseIfNotExists<DataEntryContext>
    {
        protected override void Seed(DataEntryContext context)
        {
            //read from the initial data feed to populate the data feed.
            // AppSettings InitialDataPath
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationSettings.AppSettings["InitialDataPath"]);
            var csv = new CsvReader(new StreamReader(filePath));
            var entryList = csv.GetRecords<DataEntry>();

            foreach (DataEntry entry in entryList)
            {
                context.Entries.Add(entry);
            }

            context.SaveChanges();
        }
    }

    public class DataEntryEmptyDBContextInitializer : DropCreateDatabaseAlways<DataEntryEmptyContext>
    {
    }
}