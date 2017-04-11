using DocSearch2._1.Models;
using DocSearch2._1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    public class FolderController : Controller
    {
        private IFolderRepository repository = null;

        public FolderController() {
            this.repository = new FolderRepository();
        }

        //keep for future testing
        /*
        public FolderController(IFolderRepository repository) {
            this.repository = repository;
        }
        */
        
        
        // GET: Folder
        public ActionResult Index([Bind(Prefix = "ClientId")] string Number)
        {
            tbl_Folder folder = repository.SelectByNumber(Number);

            TempData["Client_Name"] = folder.Name;
            TempData["Client_Id"] = folder.Number;

            return RedirectToAction("Index", "PublicVM", new { publicId = folder.Folder_ID });
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();

            base.Dispose(disposing);
        }
    }
}