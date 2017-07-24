﻿using DocSearch2._1.Models;
using DocSearch2._1.Repositories;
using System.Web.Mvc;

namespace DocSearch2._1.Controllers
{
    //This controller is mainly used to take in the initial request info and role, and sending the user the appropriate view through the main controller (PublicVMController)
    //***this controller may not honestly be needed in the end, and the PublicVM can do the functionality here too.
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

        //currently the role is coming as a query value, needs to be a role check through better security
        // GET: Folder
        public ActionResult Index([Bind(Prefix = "ClientId")] string Number, string Role = "")
        {
            tbl_Folder folder = null;

            try {
                folder = repository.SelectByNumber(Number);
            } catch {
                TempData["importance"] = true;
                return RedirectToAction("Index", "ErrorHandler", null);
            }

            if (folder == null)
            {
                TempData["Client_Id"] = Number;
                TempData["error_info"] = "The client does not exist.";
                TempData["importance"] = false;

                return RedirectToAction("Index", "ErrorHandler", null);
            }
            else {
                if (HttpContext.User.IsInRole("IT-ops"))
                {
                    TempData["RoleButton"] = "Admin";

                    if (Role == "Admin")
                    {
                        TempData["Role"] = "Admin";
                        TempData["RoleButton"] = "Client";
                    }
                    else
                    {
                        TempData["Role"] = "Client";
                    }
                }
                else
                {
                    TempData["Role"] = "Client";
                }

                TempData["Client_Name"] = folder.Name;
                TempData["Client_Id"] = folder.Number;
                TempData["Folder_Id"] = folder.Folder_ID; //should be a better way than carrying this variable around
            }

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