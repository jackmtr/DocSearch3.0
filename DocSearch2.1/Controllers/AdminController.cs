using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID)
        {
            return View();
        }
    }
}