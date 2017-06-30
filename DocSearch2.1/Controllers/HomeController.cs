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
        public ActionResult Index()
        {
            return View();
        }
    }
}