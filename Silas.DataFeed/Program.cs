using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using CsvHelper;
using Silas.Domain;

namespace Silas.DataFeed
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //run an api call to the web api layer every second to create stream of hits
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           ConfigurationSettings.AppSettings["FeedDataPath"]);
            var csv = new CsvReader(new StreamReader(filePath));
            IEnumerable<DataEntry> entryList = csv.GetRecords<DataEntry>();

            foreach (DataEntry entry in entryList)
            {
                entry.DateTime = DateTime.Now;
                HttpResponseMessage response = client.PostAsJsonAsync("api/livedata", entry).Result;
                Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.RequestMessage);
                Thread.Sleep(1000);
            }
        }
    }
}