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
        private static int prevPageAmount;

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
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string subNav = null, string prevNav = null, string filter = null, string searchTerm = null, string IssueYearMinRange = null, string IssueYearMaxRange = null, int page = 1, int pageSize = 15, bool Admin = false)
        {
            if (System.Web.HttpContext.Current.Session["Role"] as String == "Admin")
            {
                Admin = true;

                TempData["Role"] = "admin";

            } //checking for admin, this is temporary until a better auth checkelse
            else {
                TempData["Role"] = "client";
            }

            //**GLOBAL VARIABLES
            //TempData can be used to send data between controllers and views through one request, .keep() is used to continue keeping after one request
            //persist client name, id, search term, inputted dates
            TempData.Keep("Client_Name");
            TempData.Keep("Client_Id");        
            //***Pseudo save state immitation
            TempData.Keep("SearchTerm");
            TempData.Keep("YearRange");

            //ViewData["goodSearch"] = false means seachterm will return an empty result
            ViewData["goodSearch"] = true;
            //ViewData["currentNav"] used to populate view's link route value for subNav, which in turn populates subNav variable.  Used to save subnav state
            ViewData["currentNav"] = null;

            //declare and instantiate the original full PublicVM data for the client
            IEnumerable<PublicVM> publicModel = null;


            //**POPULATING MAIN MODEL, second conditional is for no doc reference documents, a unique westland condition
            publicModel = publicRepository
                            .SelectAll(Folder_ID, TempData["Role"].ToString());
            
            publicModel = publicModel.Where(n => n.EffectiveDate != null || n.EffectiveDate == null && n.RefNumber == null || n.EffectiveDate == null && n.RefNumber != null); //need better queries|| n.DocumentType_ID == 13 was removed bc looks to be redundant now with fixes
            //should combine the above LINQ statements when done testing and development
            //should admin neglect this line?

            publicModel = publicModel.GroupBy(x => x.Document_ID).Select(x => x.First());

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
                if (TempData["Role"].ToString() == "admin")
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

                //Bug in system when trying to retrieve a ranked doc in the model (i.e. 11th-20th docs due to pagination).
                //Current pagination reads the search and filters, so possibility it reads a rank thats not available anymore.
                //Current solution: reset the page value when needed
                if ((int)ViewData["currentRecordsCount"] < ((pageSize * page) - (pageSize - 1 ))) {
                    page = 1;
                }

                //give the page amount change some state saving for filters
                if (prevPageAmount != 0 && prevPageAmount != pageSize) {
                    filter = prevFilter;
                    prevFilter = "";
                };
                prevPageAmount = pageSize;

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

                else if (filter == "method")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.Method)
                                       .ToPagedList(page, pageSize);
                    }
                    else
                    {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.Method)
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

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.RefNumber)
                                       .ToPagedList(page, pageSize);
                    }
                    else {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.RefNumber)
                                         .ToPagedList(page, pageSize);
                    }

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

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.EffectiveDate)
                                       .ToPagedList(page, pageSize);
                    }
                    else {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.EffectiveDate)
                                         .ToPagedList(page, pageSize);
                    }
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

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.Originator)
                                       .ToPagedList(page, pageSize);
                    }
                    else {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.Originator)
                                         .ToPagedList(page, pageSize);
                    }

                    prevFilter = filter;
                }
                else if (filter == "reason")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.Reason)
                                       .ToPagedList(page, pageSize);
                    }
                    else
                    {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.Reason)
                                         .ToPagedList(page, pageSize);
                    }

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

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.Supplier)
                                       .ToPagedList(page, pageSize);
                    }
                    else {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.Supplier)
                                         .ToPagedList(page, pageSize);
                    }

                    prevFilter = filter;
                }
                else if (filter == "description")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.Description)
                                       .ToPagedList(page, pageSize);
                    }
                    else
                    {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.Description)
                                         .ToPagedList(page, pageSize);
                    }

                    prevFilter = filter;
                }
                else if (filter == "file")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }

                    //if (sortAscending)
                    //{
                    //    publicModel = publicModel
                    //                   .OrderBy(r => r.FileType)
                    //                   .ToPagedList(page, pageSize);
                    //}
                    //else
                    //{
                    //    publicModel = publicModel
                    //                     .OrderByDescending(r => r.FileType)
                    //                     .ToPagedList(page, pageSize);
                    //}

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.FileExtension)
                                       .ToPagedList(page, pageSize);
                    }
                    else
                    {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.FileExtension)
                                         .ToPagedList(page, pageSize);
                    }

                    prevFilter = filter;
                }
                else if (filter == "documentId")
                {
                    if (prevFilter == filter)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderBy(r => r.Document_ID)
                                       .ToPagedList(page, pageSize);
                    }
                    else
                    {
                        publicModel = publicModel
                                         .OrderByDescending(r => r.Document_ID)
                                         .ToPagedList(page, pageSize);
                    }

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

                    if (sortAscending)
                    {
                        publicModel = publicModel
                                       .OrderByDescending(r => r.IssueDate)
                                       .ToPagedList(page, pageSize);
                    }
                    else {
                        publicModel = publicModel
                                         .OrderBy(r => r.IssueDate)
                                         .ToPagedList(page, pageSize);
                    }

                    prevFilter = filter;
                }
                //**ENDING FILTERING OF MODEL**

                if (publicModel != null)
                {
                    //
                    ViewData["SortOrder"] = sortAscending;
                    //
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
                    ViewData["SortOrder"] = sortAscending;
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
                            .Where(y => y.IssueDate != null) //does this make business sense?
                            .OrderBy(r => r.IssueDate)
                                    .First()
                                        .IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture);
            }
            else {
                year = model
                            .Where(y => y.IssueDate != null) //does this make business sense?
                            .OrderByDescending(r => r.IssueDate)
                                    .First()
                                        .IssueDate.Value.ToString("yyyy", CultureInfo.InvariantCulture);
            }

            return year;
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
    }
}