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
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string searchTerm = null)
        {
            TempData.Keep("Person_Name");

            IEnumerable<PublicVM> publicModel = repository
                .SelectAll(Folder_ID)
                .Where(r => searchTerm == null || r.Description.Contains(searchTerm));
            //need to greatly refine this search feature

            if (publicModel != null)
            {
                return View(publicModel);
            }
            else {

                return HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();

            base.Dispose(disposing);
        }
    }
}