using System.Web.Mvc;
using System.Web.Routing;

namespace DocSearch2._1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilter(GlobalFilterCollection filters) {

            filters.Add(new HandleErrorAttribute());
        }
    }
}
