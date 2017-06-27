using DocSearch2._1.Repositories;
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using System.Globalization;
using DocSearch2._1.Models;
using System.IO;
using System.IO.Compression;

namespace DocSearch2._1.Controllers
{
    public class AdminController : Controller
    {
        private IFolderRepository repository = null;
        private IDocumentRepository documentRepository = null;
        private IPublicRepository publicRepository = null;

        private WASEntities _db = new WASEntities();

        //private static string directoryPath = @"C:\Users\jcheng\Downloads";

        public AdminController()
        {
            this.repository = new FolderRepository();
            this.documentRepository = new DocumentRepository();
            //public repo for publicVM actions
            this.publicRepository = new PublicRepository();
        }

        [HttpGet]
        public ActionResult AdminOptions(FormCollection form) {
            //do i need this action?
            return View();
        }

        // GET: Admin
        [OutputCache(NoStore = true, Duration = 0)] //prevents caching
        public ActionResult Index([Bind(Prefix = "publicId")] string Folder_ID, string IssueYearMinRange = null, string IssueYearMaxRange = null, int page = 1)
        {
            IEnumerable<PublicVM> publicModel = null;

            TempData.Keep("Role");
            TempData["Role"] = "Admin";

            ViewData["goodSearch"] = true;

            publicModel = publicRepository
                .SelectAll(Folder_ID, "admin");

            //**Populating the navbar, put into function
            populateNavBar(publicModel);

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
            if (!Request.IsAjaxRequest())
            {
                //creating the options for the dropdown list
                //doesnt look like I needed two variables to hold this list
                TempData["YearRange"] = YearRangePopulate(IssueYearMinRange, IssueYearMaxRange);
            }

            publicModel = publicModel.ToPagedList(page, 50);

            return View(publicModel);
        }

        [HttpGet]
        public ActionResult Edit([Bind(Prefix = "publicId")] string Folder_ID, string[] EditList) {

            List<PublicVM> publicModel = null;

            publicModel = publicRepository.SelectAll(Folder_ID, "admin").Where(doc => EditList.Contains(doc.Document_ID.ToString())).GroupBy(x => x.Document_ID).Select( x => x.First()).ToList();

            return PartialView("_EditTable", publicModel);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Prefix = "publicId")] string Folder_ID, List<PublicVM> updatedEditList) {

            //updatedEditList.ForEach( x=> x.)
            //updatedEditList.ForEach(

            //        //tbl_Document doc = documentRepository.SelectById(i => i.
            //    );

            if (ModelState.IsValid) {

                foreach (PublicVM pvm in updatedEditList) {

                    tbl_Document modDoc = documentRepository.SelectById(pvm.Document_ID.ToString());
                    modDoc.Issue_DT = pvm.IssueDate;
                    modDoc.Description = pvm.Description;
                    modDoc.Method = pvm.Method;
                    modDoc.Originator = pvm.Originator;
                    modDoc.Reason = pvm.Reason;
                    modDoc.Recipient = pvm.Recipient;
                    modDoc.Active_IND = pvm.Hidden;

                    _db.Entry(modDoc).State = System.Data.Entity.EntityState.Modified;
                    
                    //if (!documentRepository.SaveChanges(modDoc)) {


                    //}

                    //tbl_Document doc = AutoMapper.Mapper.Map<tbl_Document>(pvm);
                    //documentRepository.Update(doc);
                    //documentRepository.Save();
                }
                _db.SaveChanges();
            }

            return RedirectToAction("Index", "PublicVM", new { publicId = Folder_ID });
            //return View();
        }

        [HttpGet]
        //action should eventually be moved into PublicVMController for a role specific button to address
        //Downloads all documents pertaining to a specific client ID
        public ActionResult DownloadAllDocuments([Bind(Prefix = "ClientId")] string Number)
        {

            tbl_Folder folder = repository.SelectByNumber(Number);

            //gets all documents for one folder_id
            IEnumerable<tbl_Document> files = documentRepository.SelectAll(folder.Folder_ID.ToString());

            byte[] result; //blank byte array, ready to be used to filled with the tbl_document.ArchivedFile bytes

            //opening a stream to allow data to be moved
            using (var zipArchiveMemoryStream = new MemoryStream())
            {

                //creating a zipArchive obj to be used and disposed of
                using (var zipArchive = new ZipArchive(zipArchiveMemoryStream, ZipArchiveMode.Create, true))
                {

                    foreach (var file in files)
                    {
                        if (file.ArchivedFile != null)
                        {
                            //according to Ramin, creation of an ArchivedFile and Submitting an ArchivedFile are different steps, so there could be 'dirty' records/documents in WAS db that has no ArchivedFile Fields records
                            var zipEntry = zipArchive.CreateEntry(file.Document_ID.ToString() + "." + file.FileExtension); //creates a unit of space for the individual file to be placed in

                            using (var entryStream = zipEntry.Open())
                            {
                                using (var tmpMemory = new MemoryStream(file.ArchivedFile))
                                {
                                    tmpMemory.CopyTo(entryStream); //copies the data into the unit space
                                }
                            }
                        }
                    }
                }

                zipArchiveMemoryStream.Seek(0, SeekOrigin.Begin); //not exactly sure what this does, I think it pertains to the ziped folders ordering
                result = zipArchiveMemoryStream.ToArray(); //I think this is the ziped item
            }

            return new FileContentResult(result, "application/zip") { FileDownloadName = "AllArchivedFilesFor_" + Number + ".zip" };
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
        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            publicRepository.Dispose();
            documentRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}