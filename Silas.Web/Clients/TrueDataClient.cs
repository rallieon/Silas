using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Web;
using Silas.Domain;

namespace Silas.Web.Clients
{
    public class TrueDataClient : IDataClient
    {
        public IEnumerable<DataEntry> GetData()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8080/");

            HttpResponseMessage resp = client.GetAsync("api/truedata").Result;
            resp.EnsureSuccessStatusCode();

            return resp.Content.ReadAsAsync<IEnumerable<DataEntry>>().Result;
        }
    }
}