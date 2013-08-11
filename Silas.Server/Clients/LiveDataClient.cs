using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Silas.Forecast.Models;

namespace Silas.Web.Clients
{
    public class LiveDataClient : IDataClient
    {
        public DataEntry GetEntryByPeriod(int period)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");

            HttpResponseMessage resp = client.GetAsync("api/livedata/" + period).Result;
            resp.EnsureSuccessStatusCode();

            return resp.Content.ReadAsAsync<DataEntry>().Result;
        }

        public IEnumerable<DataEntry> GetData()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");

            HttpResponseMessage resp = client.GetAsync("api/livedata").Result;
            resp.EnsureSuccessStatusCode();

            IEnumerable<DataEntry> results = resp.Content.ReadAsAsync<IEnumerable<DataEntry>>().Result;

            return results.OrderBy(e => e.Period);
        }

        public IEnumerable<DataEntry> GetData(int numberOfRecords)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");

            HttpResponseMessage resp = client.GetAsync("api/livedata").Result;
            resp.EnsureSuccessStatusCode();

            IEnumerable<DataEntry> results = resp.Content.ReadAsAsync<IEnumerable<DataEntry>>().Result;

            return results.OrderBy(e => e.Period).Take(numberOfRecords);
        }
    }
}