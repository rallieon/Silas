using System;
using System.Web.Http;
using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;

namespace Silas.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string url = "http://localhost:8080";

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }

        private class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                var signalRConfig = new HubConfiguration {EnableCrossDomain = true, EnableDetailedErrors = true};
                app.MapHubs(signalRConfig);

                var webAPIConfig = new HttpConfiguration();
                webAPIConfig.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{period}"
                    );

                app.UseWebApi(webAPIConfig);

                //setup DI
                var builder = new ContainerBuilder();
                builder.Reg
            }
        }
    }
}