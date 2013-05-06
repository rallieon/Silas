using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Silas.API.Models;

namespace Silas.API
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

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
        }
    }
}