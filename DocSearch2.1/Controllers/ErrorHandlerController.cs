using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    public class ErrorHandlerController : Controller
    {
        // GET: ErrorHandler
        public ActionResult Index()
        {
            TempData.Keep("Client_Id");
            TempData.Keep("error_info");
            TempData.Keep("importance");
            //TempData.Keep("");
            //TempData.Keep("");
            //TempData.Keep("");

            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}