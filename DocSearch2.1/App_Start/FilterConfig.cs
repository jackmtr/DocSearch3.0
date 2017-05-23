using System.Web.Mvc;
using System.Web.Routing;

namespace DocSearch2._1
{
    //dont think I will be using
    //used to put global filters into all query searches iirc
    public class FilterConfig
    {
        public static void RegisterGlobalFilter(GlobalFilterCollection filters) {

            filters.Add(new HandleErrorAttribute());
        }
    }
}
