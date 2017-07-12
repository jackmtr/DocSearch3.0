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
        private IPublicRepository publicRepository = null; //public function repository
        private IDocumentRepository documentRepository = null; //document function repository
        private static bool sortAscending = true; //static var for rememebering previous sort order
        private static string prevFilter; //static var for remembering previous navbar item
        private static DateTime today = DateTime.Today; //should this be static?


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
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string subNav = null, string prevNav = null, string filter = null, string searchTerm = null, string IssueYearMinRange = null, string IssueYearMaxRange = null, /*int page = 1, int pageSize = 300,*/ bool Admin = false, string IssueMonthMinRange = null, string IssueMonthMaxRange = null)
        {
            //**GLOBAL VARIABLES
            if (System.Web.HttpContext.Current.Session["Role"] as String == "Admin") //checking for admin, this is temporary until a better auth check
            {
                Admin = true;
                TempData["Role"] = "Admin"; //Will be used in view to access more controls
            }
            else {
                TempData["Role"] = "Client"; //not really used, might be able to be removed
            }

            //TempData can be used to send data between controllers and views through one request, .keep() is used to continue keeping after one request
            //persist client name, id, search term, inputted dates
            TempData.Keep("Client_Name");
            TempData.Keep("Client_Id");
            TempData.Keep("Folder_Id");
            //***Pseudo save state immitation
            TempData.Keep("SearchTerm");
            TempData.Keep("YearRange"); //will carry an array with allowable issue date years for custom dropdown list

            //ViewData["goodSearch"] = false means seachterm will return an empty result
            ViewData["goodSearch"] = true;
            //ViewData["currentNav"] used to populate view's link route value for subNav, which in turn populates subNav variable.  Used to save subnav state
            ViewData["currentNav"] = null;

            DateTime issueDateMin = today.AddYears(-1); //appropriate place?
            DateTime issueDateMax = today; //appropriate place?

            //declare and instantiate the original full PublicVM data for the client
            IEnumerable<PublicVM> publicModel = null;


            //**POPULATING MAIN MODEL, second conditional is for no doc reference documents, a unique westland condition
            try {
                publicModel = publicRepository
                                .SelectAll(Folder_ID, TempData["Role"].ToString())
                                    .Where(n => n.EffectiveDate != null || n.EffectiveDate == null && n.RefNumber == null || n.EffectiveDate == null && n.RefNumber != null)
                                        .GroupBy(x => x.Document_ID)
                                            .Select(x => x.First());
            } catch {
                return View("Errors");
            }


            //should only be run on initial load of page
            if (!Request.IsAjaxRequest()) {
                //creating the options for the dropdown list
                //****
                TempData["YearRange"] = YearRangePopulate(RetrieveYear(publicModel, true), RetrieveYear(publicModel, false));
            }


            if (IssueYearMaxRange == null || IssueYearMaxRange == "") {

                if (IssueYearMinRange == null) {
                    IssueYearMinRange = today.AddYears(-1).Year.ToString();
                }

                int yearInput = Int32.Parse(IssueYearMinRange);

                if (yearInput > 1950)
                {
                    yearInput = yearInput - DateTime.Now.Year;
                }

                issueDateMin = today.AddYears(yearInput);
            }
            else if ((IssueYearMinRange != null && IssueYearMinRange != "") && (IssueYearMaxRange != null && IssueYearMaxRange != ""))
            {
                //custom dates
                issueDateMin = FormatDate(IssueYearMinRange, IssueMonthMinRange, true);
                issueDateMax = FormatDate(IssueYearMaxRange, IssueMonthMaxRange, false);
            }
            else if ((IssueYearMinRange == null || IssueYearMinRange == "") && (IssueYearMaxRange != null && IssueYearMaxRange != ""))
            {
                //no input for min date under CUSTOM inputs
                //min would be oldest value
                issueDateMin = DateTime.ParseExact(String.Format("1/01/1985"), "d", CultureInfo.InvariantCulture);
                issueDateMax = DateTime.ParseExact(String.Format("{1}/31/{0}", IssueYearMaxRange, IssueMonthMaxRange), "d", CultureInfo.InvariantCulture);
            }
            else
            {
                issueDateMin = today.AddYears(-1);
                issueDateMax = today;
                //dont think this should ever get hit
            }

            //count of total records unfiltered of this client
            ViewData["allRecordsCount"] = publicModel.Count();

            //**Populating the navbar, put into function
            populateNavBar(publicModel);

            if (Request.IsAjaxRequest())
            {
                //**STARTING ACTUAL FILTERING/SORTING OF MODEL**

                //*filtering by category/doctype/policy
                if (subNav != null)
                {
                    publicModel = subNavFilter(publicModel, subNav, prevNav);
                }

                //*filtering by date and search conditions
                if (TempData["Role"].ToString() == "Admin")
                {
                    //checks if the date filter and search term will return any results
                    if (!publicModel.Any(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax) && (searchTerm == null || r.Description.ToLower().Contains(searchTerm.ToLower()))))
                    {
                        //ViewData["goodSearch"] = false means no records is found
                        ViewData["goodSearch"] = false;
                    }
                    else
                    {
                        publicModel = publicModel.Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax) &&
                            (searchTerm == null || ((bool)ViewData["goodSearch"] ? r.Description.ToLower().Contains(searchTerm.ToLower()) == true : true)));

                        TempData["SearchTerm"] = searchTerm;
                    }
                    //may want to widen results if goodSearch is false
                }
                else {
                    //checks if the date filter and search term will return any results
                    if (!publicModel.Any(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax) && (searchTerm == null || r.Description.ToLower().Contains(searchTerm.ToLower()))))
                    {
                        publicModel = publicModel.Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax) && (searchTerm == null || r.Description.ToLower().Contains(searchTerm.ToLower())));

                        //ViewData["goodSearch"] = false means no records is found
                        ViewData["goodSearch"] = false;
                    }
                    else
                    {
                        publicModel = publicModel.Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax) &&
                            (searchTerm == null || ((bool)ViewData["goodSearch"] ? r.Description.ToLower().Contains(searchTerm.ToLower()) == true : true)));

                        TempData["SearchTerm"] = searchTerm;
                    }
                }

                ViewData["currentRecordsCount"] = publicModel.Count();

                //if ((int)ViewData["currentRecordsCount"] < ((pageSize * page) - (pageSize - 1))) {
                //    page = 1;
                //}

                //*sorting data
                publicModel = FilterModel(publicModel, filter, prevFilter/*, page, pageSize*/);

                //**ENDING FILTERING OF MODEL**

                if (publicModel != null)
                {

                    ViewData["SortOrder"] = sortAscending;

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
                    
                    ViewData["SortOrder"] = sortAscending;
                    publicModel = publicModel
                                    .OrderByDescending(r => r.IssueDate)
                                        .Where(r => (r.IssueDate >= issueDateMin) && (r.IssueDate <= issueDateMax))
                                            .ToList();
                                            //.ToPagedList(page, pageSize);
                    //if filtered model is already empty at start, i still need the client id

                    ViewData["currentRecordsCount"] = publicModel.Count();

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

            if (file == null) {
                ViewData["repositoryRequestDocId"] = id;

                return View("Errors");
            }

            if (file.ArchivedFile.Length < 100)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Unable to open, File Size: 0 mb');window.open('','_self').close();</script>");
            }

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

            if (file.ArchivedFile == null) {
                ViewData["repositoryRequestDocId"] = id;

                return PartialView("_FileDisplay");
            }

            return File(file.ArchivedFile, MimeType);
        }

        protected override void Dispose(bool disposing)
        {
            publicRepository.Dispose();
            documentRepository.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Method to return a collection of years within two year inputs
        /// </summary>
        /// <param name="IssueYearMinRange">DateTime of model's issue date lower range</param>
        /// <param name="IssueYearMaxRange">DateTime of model's issue date upper range</param>
        /// <returns>An IList Collection of years within the two parameter's years</returns>
        private IList<SelectListItem> YearRangePopulate(DateTime IssueYearMinRange, DateTime IssueYearMaxRange) {

            IList<SelectListItem> years = new List<SelectListItem>();

            for (int i = IssueYearMinRange.Year; i <= IssueYearMaxRange.Year; i++) {
                SelectListItem year = new SelectListItem();
                year.Text = year.Value = i.ToString();
                years.Add(year);
            }

            return years;
        }

        /// <summary>
        /// Method used to find the first or last year within the PublicVM Model
        /// </summary>
        /// <param name="model">PublicVM model that will be used, should use before any filters</param>
        /// <param name="ascending">True means it will find the year of the lowest issue date, vice versa for False</param>
        /// <returns>DateTime of an outer range of the model's IssueDate column</returns>
        private DateTime RetrieveYear(IEnumerable<PublicVM> model, bool ascending) {

            DateTime date;

            if (ascending) {
                date = model
                        .Where(y => y.IssueDate != null)
                            .OrderBy(r => r.IssueDate)
                                .First()
                                    .IssueDate.Value;
            }
            else
            {
                date = model
                        .Where(y => y.IssueDate != null)
                            .OrderByDescending(r => r.IssueDate)
                                .First()
                                    .IssueDate.Value;
            }

            return date;
        }

        private void populateNavBar(IEnumerable<PublicVM> model)
        {
            IEnumerable<PublicVM> nb = model
                                        .OrderBy(e => e.CategoryName)
                                            .GroupBy(e => e.CategoryName)
                                                .Select(g => g.First());

            List<NavBar> nbl = new List<NavBar>();

            foreach (PublicVM pvm in nb)
            {

                NavBar nbitem = new NavBar();

                nbitem.CategoryName = pvm.CategoryName;

                foreach (PublicVM pp in model
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
            ViewBag.PolicyNavBar = model
                                    .Where(e => e.EffectiveDate != null) //needs to be removed because (T) ref# and (F) EffDate needs to be brought through model, but this criteria should not be used to populate the navbar policies
                                        .OrderBy(e => e.RefNumber)
                                            .GroupBy(e => e.RefNumber)
                                                .Select(g => g.First().RefNumber);
        }

        private IEnumerable<PublicVM> subNavFilter(IEnumerable<PublicVM> model, string subNav, string prevNav)
        {
            switch (subNav)
            {
                case "category":
                    model = model.Where(r => r.CategoryName == prevNav);
                    break;
                case "doctype":
                    model = model.Where(r => r.DocumentTypeName == prevNav);
                    break;
                case "policy":
                    model = model.Where(r => r.RefNumber == prevNav);
                    break;
            }

            ViewData["currentNav"] = subNav;
            ViewData["currentNavTitle"] = prevNav;

            return model;
        }

        private IEnumerable<PublicVM> FilterModel(IEnumerable<PublicVM> model, string filter, string prevFilter/*, int page, int pageSize*/)
        {
            if (filter == null || filter == "")
            {
                sortAscending = true;
            }
            else {
                if (prevFilter == filter || prevFilter == null)
                {
                    sortAscending = !sortAscending;
                }
                else
                {
                    sortAscending = true;
                }
            }

            switch (filter)
            {

                case "document":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.DocumentTypeName)
                                        .ToList();
                                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.DocumentTypeName)
                                        .ToList();
                                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "method":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Method)
                                        .ToList();
                                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Method)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "policy":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.RefNumber)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.RefNumber)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "effective":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.EffectiveDate)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.EffectiveDate)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "originator":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Originator)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Originator)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "reason":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Reason)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Reason)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "supplier":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Supplier)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Supplier)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "description":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Description)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Description)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "file":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.FileExtension)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.FileExtension)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "documentId":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Document_ID)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Document_ID)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                case "hidden":

                    if (sortAscending)
                    {
                        model = model
                                    .OrderBy(r => r.Hidden)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                    .OrderByDescending(r => r.Hidden)
                                        .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;

                default:

                    if (sortAscending)
                    {
                        model = model
                                .OrderByDescending(r => r.IssueDate)
                                    .ToList();
                        //.ToPagedList(page, pageSize);
                    }
                    else
                    {
                        model = model
                                .OrderBy(r => r.IssueDate)
                                    .ToList();
                        //.ToPagedList(page, pageSize);
                    }

                    break;
            }

            prevFilter = filter;

            return model;
        }

        private DateTime FormatDate(string inputYear, string inputMonth, bool isStartingDate) {

            int year = Int32.Parse(inputYear);
            int month = 0;

            if (inputMonth != null)
            {
                month = Int32.Parse(inputMonth) + 1;
            }
            else {
                if (isStartingDate)
                {
                    month = 2;
                }
                else {
                    month = 13;
                }
            }

            if (month == 13)
            {
                year = Int32.Parse(inputYear) + 1;
                month = 1;
            }

            if (isStartingDate)
            {
                return new DateTime(year, month, 1).AddMonths(-1);
            }
            else {
                DateTime endingDate = new DateTime(year, month, 1).AddDays(-1);

                return endingDate;
            }
        }
    }
}