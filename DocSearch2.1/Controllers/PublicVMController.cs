using DocSearch2._1.Repositories;
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    public class PublicVMController : Controller
    {
        private IPublicRepository repository = null;

        public PublicVMController() {
            this.repository = new PublicRepository();
        }

        //keep for future testing
        /*
        public PublicVMController(IPublicRepository repository) {

            this.repository = repository;
        }
        */

        // GET: PublicVM
        [HttpGet] // dunno if need this: 
        public ActionResult Index(string publicId, string searchTerm = null)
        {
            TempData.Keep("Person_Name");

            IEnumerable<PublicVM> publicModel = repository
                .SelectAll(publicId)
                .Where(r => searchTerm == null || r.Description.Contains(searchTerm));
            //need to greatly refine this search feature

            return View(publicModel);
        }
    }
}