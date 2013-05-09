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

            //setup data directory
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());

            Database.SetInitializer(new LiveDataContextInitializer());
            using (var context = new LiveDataContext())
            {
                context.Database.Initialize(true);
            }

            Database.SetInitializer(new TrueDataContextInitializer());
            using (var context = new TrueDataContext())
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
