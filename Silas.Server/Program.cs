using System;
using System.Data.Entity;
using System.IO;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Silas.Server.DB;

namespace Silas.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            //setup data directory
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());

            Database.SetInitializer(new LiveDataContextInitializer());
            using (var context = new LiveDataContext())
            {
                context.Database.Initialize(true);
            }

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new {id = RouteParameter.Optional});

            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}