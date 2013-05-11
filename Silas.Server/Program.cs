using System;
using System.Data.Entity;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
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

            config.Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}");
            config.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}