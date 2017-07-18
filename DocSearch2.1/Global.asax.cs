using DocSearch2._1.Models;
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal; //trying for security
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DocSearch2._1
{
    public class WindowAuth {
        public static string WindowLoginName { get; set; }
    }
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WindowAuth.WindowLoginName = WindowsIdentity.GetCurrent().Name;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilter(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
