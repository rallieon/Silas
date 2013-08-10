using System;
using System.Data.Entity;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Silas.Server.DB;

namespace Silas.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://localhost:8080";

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                // Turn cross domain on 
                var config = new HubConfiguration { EnableCrossDomain = true, EnableDetailedErrors = true};

                // This will map out to http://localhost:8080/signalr by default
                app.MapHubs(config);
            }
        }

        public class MyHub : Hub
        {
            public string Send(string name, string message)
            {
                Console.WriteLine("Name = " + name + ":Message=" + message);
                Clients.All.addMessage(name, message);
                return message;
            }
        }
    }
}