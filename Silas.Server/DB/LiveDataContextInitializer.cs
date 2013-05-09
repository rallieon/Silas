using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using CsvHelper;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class LiveDataContextInitializer: DropCreateDatabaseAlways<LiveDataContext>
    {
        protected override void Seed(LiveDataContext context)
        {
            //only read the first one hundred records from the true data feed.
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationSettings.AppSettings["TrueDataPath"]);
            var csv = new CsvReader(new StreamReader(filePath));
            var entryList = csv.GetRecords<DataEntry>();
            int i = 0;

            foreach (DataEntry entry in entryList)
            {
                if(i < 100)
                    context.Entries.Add(entry);

                i++;
            }

            context.SaveChanges();
        }
    }
}