using DocSearch2._1.Repositories;
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Globalization;
using DocSearch2._1.Models;

namespace DocSearch2._1.Controllers
{
    public class PublicVMController : Controller
    {
        private IPublicRepository repository = null;
        private IDocumentRepository repository1 = null;
        private static bool sortAscending = true;
        private static string prevFilter;

        public PublicVMController() {
            this.repository = new PublicRepository();
            this.repository1 = new DocumentRepository();
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
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string category = null, string policyNumber = null, string documentTypeName = null, string filter = null, string searchTerm = null, string IssueDateMinRange = "01/15/1990", string IssueDateMaxRange = "04/11/2017", int page = 1)
        {
            IEnumerable<PublicVM> publicModel = null;

            TempData.Keep("Client_Name");
            TempData.Keep("Client_Id");

            if ((category != null) || (documentTypeName != null) || (policyNumber != null) || (filter != null)) {
                if (category != null)
                {
                    publicModel = repository
                        .SelectAll(Folder_ID)
                        .Where(r => r.CategoryName == category);
                    //.ToPagedList(page, 10);
                }
                else if (documentTypeName != null)
                {
                    publicModel = repository
                        .SelectAll(Folder_ID)
                        .Where(r => r.DocumentTypeName == documentTypeName);
                    //.ToPagedList(page, 10);
                }
                else if (policyNumber != null)
                {
                    publicModel = repository
                        .SelectAll(Folder_ID)
                        .Where(r => r.RefNumber == policyNumber);
                    //.ToPagedList(page, 10);
                }
                else {
                    publicModel = repository
                        .SelectAll(Folder_ID);
                }
            }
            else {
                //"04/10/2017" example expected date
                DateTime issueDateMin = DateTime.ParseExact(IssueDateMinRange, "d", CultureInfo.InvariantCulture);
                DateTime issueDateMax = DateTime.ParseExact(IssueDateMaxRange, "d", CultureInfo.InvariantCulture);

                publicModel = repository
                    .SelectAll(Folder_ID)
                    .Where(r => searchTerm == null || r.Description.Contains(searchTerm))
                    .Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax));
                    //.ToPagedList(page, 10);
                //need to greatly refine this search feature
            }


            if (Request.IsAjaxRequest()) {

                if (filter == "document")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else {
                        sortAscending = true;
                    }

                    if (sortAscending) publicModel = publicModel.OrderBy(r => r.DocumentTypeName).ToPagedList(page, 10);
                    else publicModel = publicModel.OrderByDescending(r => r.DocumentTypeName).ToPagedList(page, 10);

                    prevFilter = filter;
                }
                else if (filter == "issue")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else {
                        sortAscending = true;
                    }
                    if (sortAscending) publicModel = publicModel.OrderBy(r => r.IssueDate).ToPagedList(page, 10);
                    else publicModel = publicModel.OrderByDescending(r => r.IssueDate).ToPagedList(page, 10);

                    prevFilter = filter;
                }
                else if (filter == "effective")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else {
                        sortAscending = true;
                    }

                    if (sortAscending) publicModel = publicModel.OrderBy(r => r.EffectiveDate).ToPagedList(page, 10);
                    else publicModel = publicModel.OrderByDescending(r => r.EffectiveDate).ToPagedList(page, 10);

                    prevFilter = filter;
                }
                else {
                    publicModel = publicModel.ToPagedList(page, 10);
                }

                return PartialView("_PublicTable", publicModel);
            }

            if (publicModel != null)
            {        
                IEnumerable<PublicVM> nb = publicModel.OrderBy(e => e.CategoryName).GroupBy(e => e.CategoryName).Select(g => g.First());

                List<NavBar> nbl = new List<NavBar>();

                foreach (PublicVM pvm in nb) {

                    NavBar nbitem = new NavBar();

                    nbitem.CategoryName = pvm.CategoryName;

                    foreach (PublicVM pp in publicModel.GroupBy(g => g.DocumentTypeName).Select(g => g.First())) {
                        if (pp.CategoryName == nbitem.CategoryName && !nbl.Any(s => s.DocumentTypeName.Contains(pp.DocumentTypeName))) {
                            nbitem.DocumentTypeName.Add(pp.DocumentTypeName);
                        }
                    }
                    nbl.Add(nbitem);
                }

                ViewBag.CategoryNavBar = nbl;
                ViewBag.PolicyNavBar = publicModel.OrderBy(e => e.RefNumber).GroupBy(e => e.RefNumber).Select(g => g.First().RefNumber);

                publicModel = publicModel.ToPagedList(page, 10);
                return View(publicModel);
            }
            else {

                return HttpNotFound();
            }
        }

        // Get: File
        public ActionResult FileDisplay([Bind(Prefix = "documentId")] string id) {

            var file = repository1.SelectById(id);

            string MimeType = null;

            switch (file.FileExtension.ToLower().Trim()) {

                case "pdf":
                    MimeType = "application/pdf";
                    break;

                case "gif":
                    MimeType = "image/gif";
                    break;

                case "jpg":
                    MimeType = "image/jpeg";
                    break;

                case "msg":
                    MimeType = "application/vnd.outlook";
                    break;

                case "ppt":
                    MimeType = "application/vnd.ms-powerpoint";
                    break;

                case "xls":
                case "csv":
                    MimeType = "application/vnd.ms-excel";
                    break;

                case "xlsx":
                    MimeType = "application/vnd.ms-excel.12";
                    break;

                case "doc":
                case "dot":
                    MimeType = "application/msword";
                    break;

                case "docx":
                    MimeType = "application/vnd.ms-word.document.12";
                    break;

                default:
                    MimeType = "text/html";
                    break;
            }

            return File(file.ArchivedFile, MimeType); 
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();

            base.Dispose(disposing);
        }
    }
}