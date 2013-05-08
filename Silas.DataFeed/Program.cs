using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Silas.Domain;

namespace Silas.DataFeed
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            //run an api call to the web api layer every second to create stream of hits
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationSettings.AppSettings["InitialDataPath"]);
            var csv = new CsvReader(new StreamReader(filePath));
            var entryList = csv.GetRecords<DataEntry>();

            foreach (DataEntry entry in entryList)
            {
                entry.DateTime = DateTime.Now;
                var response = client.PostAsJsonAsync("api/dataentry", entry).Result;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.RequestMessage);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
