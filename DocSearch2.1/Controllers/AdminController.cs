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

namespace DocSearch2._1.Controllers
{
    public class AdminController : Controller
    {
        private IPublicRepository publicRepository = null;

        public AdminController()
        {
            //public repo for publicVM actions
            this.publicRepository = new PublicRepository();
        }

        // GET: Admin
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

        public ActionResult Edit(string[] EditList) {

            TempData.Keep("Folder_Id");

            IEnumerable<PublicVM> publicModel = null;

            publicModel = publicRepository.SelectAll(TempData["Folder_Id"].ToString(), "admin").Where(n => n.EffectiveDate != null || n.EffectiveDate == null && n.RefNumber == null || n.EffectiveDate == null && n.RefNumber != null).Where(doc => EditList.Contains(doc.Document_ID.ToString())); ;

            return PartialView("_EditTable", publicModel);
            //return RedirectToAction("Edit1");
        }

        public ActionResult Edit1(string[] EditList)
        {
            return View();
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
    }
}