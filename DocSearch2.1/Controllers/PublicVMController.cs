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
        private IPublicRepository publicRepository = null;
        private IDocumentRepository documentRepository = null;
        private static bool sortAscending = true;
        private static string prevFilter;

        public PublicVMController() {
            //public repo for publicVM actions
            this.publicRepository = new PublicRepository();

            //doc repo for document actions
            this.documentRepository = new DocumentRepository();
        }

        //keep for future testing
        /*
        public PublicVMController(IPublicRepository repository) {

            this.repository = repository;
        }
        */

        // GET: PublicVM
        [HttpGet]
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string subNav = null, string prevNav = null, string filter = null, string searchTerm = null, string IssueYearMinRange = null, string IssueYearMaxRange = null, int page = 1, int pageSize = 20)
        {
            //**GLOBAL VARIABLES
            //TempData can be used to send data between controllers and views through one request, .keep() is used to continue keeping after one request
            //persist client name, id, search term, inputted dates
            TempData.Keep("Client_Name");
            TempData.Keep("Client_Id");        
            //***Pseudo save state immitation
            TempData.Keep("SearchTerm");
            //TempData.Keep("IssueDateMin");
            //TempData.Keep("IssueDateMax");
            TempData.Keep("YearRange");

            //ViewData["goodSearch"] = false means seachterm will return an empty result
            ViewData["goodSearch"] = true;
            //ViewData["currentNav"] used to populate view's link route value for subNav, which in turn populates subNav variable.  Used to save subnav state
            ViewData["currentNav"] = null;

            //declare and instantiate the original full PublicVM data for the client
            IEnumerable<PublicVM> publicModel = null;


            //**POPULATING MAIN MODEL, second conditional is for no doc reference documents, a unique westland condition
            publicModel = publicRepository
                            .SelectAll(Folder_ID)
                            .Where(n => n.EffectiveDate != null || n.EffectiveDate == null && n.RefNumber == null);

            //instantiating the overall min and max YEAR ranges for this client if date inputs were null, maybe combine into one conditional
            if (IssueYearMinRange == null || IssueYearMinRange == "")
            {
                IssueYearMinRange = RetrieveYear(publicModel, true);
            }

            if (IssueYearMaxRange == null || IssueYearMaxRange == "")
            {
                IssueYearMaxRange = RetrieveYear(publicModel, false);
            }

            //should only be run on initial load of page
            if (!Request.IsAjaxRequest()) {

                //creating the options for the dropdown list
                //doesnt look like I needed two variables to hold this list
                //TempData["IssueDateMin"] = TempData["IssueDateMax"] = YearRangePopulate(IssueYearMinRange, IssueYearMaxRange);
                TempData["YearRange"] = YearRangePopulate(IssueYearMinRange, IssueYearMaxRange);
            }

            //Formatting the display date into SQL likeable date type
            if (Int32.Parse(IssueYearMaxRange) < Int32.Parse(IssueYearMinRange)) {
                string temp = IssueYearMinRange;
                IssueYearMinRange = IssueYearMaxRange;
                IssueYearMaxRange = temp;
            }

            DateTime issueDateMin = DateTime.ParseExact(String.Format("01/01/{0}", IssueYearMinRange), "d", CultureInfo.InvariantCulture);
            DateTime issueDateMax = DateTime.ParseExact(String.Format("12/31/{0}", IssueYearMaxRange), "d", CultureInfo.InvariantCulture);

            //count of total records unfiltered of this client
            ViewData["allRecordsCount"] = publicModel.Count();

            //**Populating the navbar
            IEnumerable<PublicVM> nb = publicModel
                                        .OrderBy(e => e.CategoryName)
                                        .GroupBy(e => e.CategoryName)
                                        .Select(g => g.First());

            List<NavBar> nbl = new List<NavBar>();

            foreach (PublicVM pvm in nb)
            {

                NavBar nbitem = new NavBar();

                nbitem.CategoryName = pvm.CategoryName;

                foreach (PublicVM pp in publicModel
                                        .GroupBy(g => g.DocumentTypeName)
                                        .Select(g => g.First()))
                {
                    if (pp.CategoryName == nbitem.CategoryName && !nbl.Any(s => s.DocumentTypeName.Contains(pp.DocumentTypeName)))
                    {
                        nbitem.DocumentTypeName.Add(pp.DocumentTypeName);
                    }
                }
                nbl.Add(nbitem);
            }

            ViewBag.CategoryNavBar = nbl;
            ViewBag.PolicyNavBar = publicModel
                                    .OrderBy(e => e.RefNumber)
                                    .GroupBy(e => e.RefNumber)
                                    .Select(g => g.First().RefNumber);
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

                //*filtering by date and search conditions
                //checks if the date filter and search term will return any results
                //changed the search condition syntax, had a bug with pink, 10 page size, then 2015 max
                /*if (!publicModel.Any(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax)) || (searchTerm != null && !publicModel.Any(pub => pub.Description.Contains(searchTerm))))
                {*/
                if (!publicModel.Any(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax) && (searchTerm == null || r.Description.Contains(searchTerm)))){ 
                    //lets view know that no results are coming back to it
                    ViewData["goodSearch"] = false;

                    ///so inefficient, need to be redone
                    //instantiating the overall min and max YEAR ranges for this client if date inputs were null, maybe combine into one conditional
                    IssueYearMinRange = publicModel
                                            .OrderBy(r => r.IssueDate)
                                            .First().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture);
                    IssueYearMaxRange = publicModel
                                            .OrderByDescending(r => r.IssueDate)
                                            .First().IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture);
                } else {
                    publicModel = publicModel.Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax));
                    TempData["SearchTerm"] = searchTerm;
                    //can probably refine this LINQ query
                    publicModel = publicModel
                                    .Where(r => searchTerm == null || ((bool)ViewData["goodSearch"] ? r.Description.Contains(searchTerm) == true : true));
                }
                //may want to widen results if goodSearch is false


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

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                        .OrderBy(r => r.DocumentTypeName)
                                            .ToPagedList(page, pageSize);
                    }
                    else {
                        publicModel = publicModel
                                        .OrderByDescending(r => r.DocumentTypeName)
                                            .ToPagedList(page, pageSize);
                    }

                    prevFilter = filter;
                }
                else if (filter == "policy")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else {
                        sortAscending = true;
                    }

                    if (sortAscending) publicModel = publicModel
                                                        .OrderBy(r => r.RefNumber)
                                                        .ToPagedList(page, pageSize);
                    else publicModel = publicModel
                                            .OrderByDescending(r => r.RefNumber)
                                            .ToPagedList(page, pageSize);

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

                    if (sortAscending) publicModel = publicModel
                                                        .OrderBy(r => r.EffectiveDate)
                                                        .ToPagedList(page, pageSize);
                    else publicModel = publicModel
                                            .OrderByDescending(r => r.EffectiveDate)
                                            .ToPagedList(page, pageSize);

                    prevFilter = filter;
                }
                else if (filter == "originator")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else {
                        sortAscending = true;
                    }

                    if (sortAscending) publicModel = publicModel
                                                        .OrderBy(r => r.Originator)
                                                        .ToPagedList(page, pageSize);
                    else publicModel = publicModel
                                            .OrderByDescending(r => r.Originator)
                                            .ToPagedList(page, pageSize);

                    prevFilter = filter;
                }
                else if (filter == "supplier")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else {
                        sortAscending = true;
                    }

                    if (sortAscending) publicModel = publicModel
                                                        .OrderBy(r => r.Supplier)
                                                        .ToPagedList(page, pageSize);
                    else publicModel = publicModel
                                            .OrderByDescending(r => r.Supplier)
                                            .ToPagedList(page, pageSize);

                    prevFilter = filter;
                }
                else {
                    if (filter == null)
                    {
                        sortAscending = true;
                    }
                    else {
                        sortAscending = !sortAscending;
                    }

                    if (sortAscending) publicModel = publicModel
                                                        .OrderByDescending(r => r.IssueDate)
                                                        .ToPagedList(page, pageSize);
                    else publicModel = publicModel
                                            .OrderBy(r => r.IssueDate)
                                            .ToPagedList(page, pageSize);

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
                    ViewData["currentRecordsCount"] = publicModel.Count();
                    publicModel = publicModel
                                    .OrderByDescending(r => r.IssueDate)
                                    .ToPagedList(page, pageSize);

                    return View(publicModel);
                }
                else {
                    return HttpNotFound();
                }
            }
        }

        public ActionResult MiscData([Bind(Prefix = "documentId")] string Document_ID, string subNav, string prevNav) {
            //declare and instantiate the original full MiscPublicData data for the client
            MiscPublicData documentData = null;

            documentData = documentRepository
                                .GetMiscPublicData(Document_ID);

            if (documentData != null)
            {
                ViewData["currentNav"] = subNav;
                ViewData["currentNavTitle"] = prevNav;

                return PartialView(documentData);
            }
            else {
                return HttpNotFound();
            }
        }

        // Get: File
        public ActionResult FileDisplay([Bind(Prefix = "documentId")] string id) {

            var file = documentRepository.SelectById(id);

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
            publicRepository.Dispose();
            documentRepository.Dispose();

            base.Dispose(disposing);
        }

        private IList<SelectListItem> YearRangePopulate(string IssueYearMinRange, string IssueYearMaxRange)
        {

            IList<SelectListItem> years = new List<SelectListItem>();

            for (int i = Int32.Parse(IssueYearMinRange); i <= Int32.Parse(IssueYearMaxRange); i++)
            {
                SelectListItem year = new SelectListItem();
                year.Selected = false;
                year.Text = year.Value = i.ToString();
                years.Add(year);
            }

            return years;
        }

        private string RetrieveYear(IEnumerable<PublicVM> model, bool ascending)
        {
            string year;

            if (ascending)
            {
                year = model
                            .OrderBy(r => r.IssueDate)
                                    .First()
                                        .IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture);
            }
            else {
                year = model
                            .OrderByDescending(r => r.IssueDate)
                                    .First()
                                        .IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture);
            }

            return year;
        }
    }
}