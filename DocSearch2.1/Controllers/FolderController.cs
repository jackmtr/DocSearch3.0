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
        public ActionResult Index(string publicId)
        {
            tbl_Folder folder = repository.SelectByID(publicId);

            TempData["Person_Name"] = folder.Name;

            return RedirectToAction("Index", "PublicVM", new { publicId = publicId });
        }
    }
}