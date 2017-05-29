using DocSearch2._1.Models;
using DocSearch2._1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    //this controller is the inbetween for this API taking the selected WAS' db tbl_Folder.Name and give the main PublicVMController's index method the tbl_Folder.Folder_ID and some static folder info (name and id)
    //this controller may not honestly be needed in the end, and the PublicVM can do the functionality here too.
    public class FolderController : Controller
    {
        private IFolderRepository repository = null;

        public FolderController() {
            this.repository = new FolderRepository();
        }

        //keep for potential of future testing of db connection
        /*
        public FolderController(IFolderRepository repository) {
            this.repository = repository;
        }
        */
        
        
        // GET: Folder
        public ActionResult Index([Bind(Prefix = "ClientId")] string Number)
        {
            tbl_Folder folder = repository.SelectByNumber(Number);


            try
            {
                TempData["Client_Name"] = folder.Name;
                TempData["Client_Id"] = folder.Number;
            }
            catch {
                return HttpNotFound();
            }
            //redirectToAction allows controller chaining
            return RedirectToAction("Index", "PublicVM", new { publicId = folder.Folder_ID });
        }

        //Dispose any open connection when finished (db in this regard)
        protected override void Dispose(bool disposing)
        {
            repository.Dispose();

            base.Dispose(disposing);
        }
    }
}