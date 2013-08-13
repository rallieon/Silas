﻿using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Hosting;
using Ninject;
using Owin;
using Silas.Server.Broadcasters;
using Silas.Server.Hubs;

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
                var kernel = new StandardKernel();
                var resolver = new NinjectSignalRDependencyResolver(kernel);

                kernel.Bind<IForecastingDataBroadcaster>()
                    .To<ForecastingDataBroadcaster>()
                    .InSingletonScope();

                kernel.Bind<IHubConnectionContext>().ToMethod(context =>
                        resolver.Resolve<IConnectionManager>().
                            GetHubContext<ForecastingDataHub>().Clients
                    ).WhenInjectedInto<IForecastingDataBroadcaster>();

                var signalRConfig = new HubConfiguration
                    {
                        EnableCrossDomain = true,
                        EnableDetailedErrors = true,
                        Resolver = resolver
                    };
                app.MapHubs(signalRConfig);

                var webAPIConfig = new HttpConfiguration();
                webAPIConfig.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{period}"
                    );

                app.UseWebApi(webAPIConfig);
            }
        }
    }
}