using DocSearch2._1.Repositories;
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Globalization;

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
        [HttpGet] // dunno if need this, was causing issues with the search return request
        //I think the search submit is coming back as a post
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string searchTerm = null, string IssueDateMinRange = "01/15/1990", string IssueDateMaxRange = "04/11/2017", int page = 1)
        {
            TempData.Keep("Client_Name");
            TempData.Keep("Client_Id");

            //"04/10/2017" example date
            DateTime issueDateMin = DateTime.ParseExact(IssueDateMinRange, "d", CultureInfo.InvariantCulture);
            DateTime issueDateMax = DateTime.ParseExact(IssueDateMaxRange, "d", CultureInfo.InvariantCulture);

            IEnumerable<PublicVM> publicModel = repository
                .SelectAll(Folder_ID)
                .Where(r => searchTerm == null || r.Description.Contains(searchTerm))
                .Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax))
                .ToPagedList(page, 10);
            //need to greatly refine this search feature

            if (Request.IsAjaxRequest()) {
                return PartialView("_PublicTable", publicModel);
            }

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