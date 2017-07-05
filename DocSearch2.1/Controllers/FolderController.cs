﻿using DocSearch2._1.Models;
using DocSearch2._1.Repositories;
//mb temp
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
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
        public ActionResult Index([Bind(Prefix = "ClientId")] string Number, string Role)
        {
            tbl_Folder folder = repository.SelectByNumber(Number);

            System.Web.HttpContext.Current.Session["Role"] = Role; //TEMPORARY until I set up a real role checker

            try
            {
                TempData["Client_Name"] = folder.Name;
                TempData["Client_Id"] = folder.Number;
            }
            catch {
                TempData["Client_Id"] = Number;
                return View("Errors");
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