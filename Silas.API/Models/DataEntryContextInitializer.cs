using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using CsvHelper;
using Silas.Domain;

namespace Silas.API.Models
{
    public class DataEntryContextInitializer : CreateDatabaseIfNotExists<DataEntryContext>
    {
        protected override void Seed(DataEntryContext context)
        {
            //read from the initial data feed to populate the data feed.
            // AppSettings InitialDataPath
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WebConfigurationManager.AppSettings["InitialDataPath"]);
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