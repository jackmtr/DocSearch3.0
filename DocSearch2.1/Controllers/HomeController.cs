using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    public class HomeController : Controller
    {
        // Technically this code wont exist/be used in production
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}