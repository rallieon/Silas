using System;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Silas.Server.DB;

namespace Silas.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            Database.SetInitializer(new DataEntryContextInitializer());
            using (var context = new DataEntryContext())
            {
                context.Database.Initialize(true);
            }

            Database.SetInitializer(new DataEntryEmptyDBContextInitializer());
            using (var context = new DataEntryEmptyContext())
            {
                context.Database.Initialize(true);
            }

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
