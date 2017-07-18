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
    public class MyClass {
        private static string myUser;
        public static string MyVar { get; set; }
    }
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MyClass.MyVar = WindowsIdentity.GetCurrent().Name;

            //MyClass.MyVar = HttpContext.Current.User.Identity.Name;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilter(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //AutoMapper.Mapper.Initialize(cfg => cfg.CreateMap<PublicVM, tbl_Document>());
        }
    }
}
