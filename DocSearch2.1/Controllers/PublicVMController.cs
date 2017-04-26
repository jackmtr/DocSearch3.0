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

        //***Check if model dates are populating correctly

        //keep for future testing
        /*
        public PublicVMController(IPublicRepository repository) {

            this.repository = repository;
        }
        */

        // GET: PublicVM
        [HttpGet] // dunno if need this, was causing issues with the search return request
        //I think the search submit is coming back as a post
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string subNav = null, string prevNav = null, string filter = "issue", string searchTerm = null, string IssueDateMinRange = null, string IssueDateMaxRange = null, int page = 1)
        {
            //persist client name, id
            TempData.Keep("Client_Name");
            TempData.Keep("Client_Id");
            TempData.Keep("SearchTerm");

            //false means seachterm will return an empty result
            ViewData["goodSearch"] = true;
            //viewdata to save state
            ViewData["currentNav"] = null;

            //declare and instantiate the original full PublicVM data for the client
            IEnumerable<PublicVM> publicModel = null;

            publicModel = repository
                        .SelectAll(Folder_ID);

            //instantiating the overall min and max date ranges for this client if date inputs were null
            if (IssueDateMinRange == null)
            {
                TempData["IssueDateMin"] = publicModel.OrderBy(r => r.IssueDate).First().IssueDate.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                IssueDateMinRange = (string)TempData["IssueDateMin"];
            }

            if (IssueDateMaxRange == null)
            {
                TempData["IssueDateMax"] = publicModel.OrderByDescending(r => r.IssueDate).First().IssueDate.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                IssueDateMaxRange = (string)TempData["IssueDateMax"];
            }

            //Formatting the display date into SQL likeable date type
            //"04/10/2017" example expected date
            DateTime issueDateMin = DateTime.ParseExact(IssueDateMinRange, "d", CultureInfo.InvariantCulture);
            DateTime issueDateMax = DateTime.ParseExact(IssueDateMaxRange, "d", CultureInfo.InvariantCulture);

            //count of total records unfiltered of this client
            ViewData["allRecordsCount"] = publicModel.Count();

            //**Populates the navbar
            IEnumerable<PublicVM> nb = publicModel.OrderBy(e => e.CategoryName).GroupBy(e => e.CategoryName).Select(g => g.First());

            List<NavBar> nbl = new List<NavBar>();

            foreach (PublicVM pvm in nb)
            {

                NavBar nbitem = new NavBar();

                nbitem.CategoryName = pvm.CategoryName;

                foreach (PublicVM pp in publicModel.GroupBy(g => g.DocumentTypeName).Select(g => g.First()))
                {
                    if (pp.CategoryName == nbitem.CategoryName && !nbl.Any(s => s.DocumentTypeName.Contains(pp.DocumentTypeName)))
                    {
                        nbitem.DocumentTypeName.Add(pp.DocumentTypeName);
                    }
                }
                nbl.Add(nbitem);
            }

            ViewBag.CategoryNavBar = nbl;
            ViewBag.PolicyNavBar = publicModel.OrderBy(e => e.RefNumber).GroupBy(e => e.RefNumber).Select(g => g.First().RefNumber);
            //**End of navbar population data

            
            if (Request.IsAjaxRequest())
            {
                //**STARTING ACTUAL FILTERING/SORTING OF MODEL**
                //*filtering by category/doctype/policy
                if (subNav != null)
                {
                    if (subNav == "category")
                    {
                        publicModel = publicModel.Where(r => r.CategoryName == prevNav);
                        ViewData["currentNav"] = "category";
                        ViewData["currentNavTitle"] = prevNav;
                    }
                    else if (subNav == "doctype")
                    {
                        publicModel = publicModel.Where(r => r.DocumentTypeName == prevNav);
                        ViewData["currentNav"] = "doctype";
                        ViewData["currentNavTitle"] = prevNav;
                    }
                    else {
                        publicModel = publicModel.Where(r => r.RefNumber == prevNav);
                        ViewData["currentNav"] = "policyNumber";
                        ViewData["currentNavTitle"] = prevNav;
                    }
                }

                //think about combining the search and date conditions
                //*filtering by date conditions
                publicModel = publicModel.Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax));


                //*filtering by searchconditions
                //checks if the search term will return any results
                if (searchTerm != null)
                {
                    ViewData["goodSearch"] = publicModel.Any(pub => pub.Description.Contains(searchTerm));
                    TempData["SearchTerm"] = searchTerm;
                    //can probably refine this LINQ query
                    publicModel = publicModel.Where(r => searchTerm == null || ((bool)ViewData["goodSearch"] ? r.Description.Contains(searchTerm) == true : true));
                }

                //record count after category/doctype/policy/search/date filter
                ViewData["currentRecordsCount"] = publicModel.Count();

                //*sorting data
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
                    //must be filter = "issue" 
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
                //**ENDING FILTERING OF MODEL**

                if (publicModel != null)
                {
                    return PartialView("_PublicTable", publicModel);
                }
                else {
                    return HttpNotFound();
                }
            }
            else { 
                //pretty much should only be the initial synchronous load to come in here
                if (publicModel != null)
                {        
                    publicModel = publicModel.OrderByDescending(r => r.IssueDate).ToPagedList(page, 10);

                    return View(publicModel);
                }
                else {
                    return HttpNotFound();
                }
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