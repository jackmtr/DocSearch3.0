using DocSearch2._1.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    public class HomeController : Controller
    {
        //The front page to input the folder info to look up, this page isnt needed in production as WEIS should be supplying the info
        // GET: Home
        //[AuthorizeUser(AccessLevel = "Create")]
        public ActionResult Index()
        {
            //TempData["UserName"] = MyClass.MyVar;
            //TempData["UserName1"] = HttpContext.User.Identity.Name;


            TempData["Role"] = "Unautherized";

            if (HttpContext.User.IsInRole("IT-ops"))
            {
                TempData["Role"] = "Admin";
            }
            else if (HttpContext.User.IsInRole("Domain Users"))
            {
                TempData["Role"] = "Client";
            }
            else
            {
                TempData["Role"] = "Unautherized";
            }

            return View();
        }
    }
}