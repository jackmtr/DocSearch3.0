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
        //private IDocumentRepository documentRepository = null;
        //possibly tempory
        //private IPublicRepository publicRepository = null;
        private static string directoryPath = @"C:\Users\jcheng\Downloads";

        public FolderController() {
            this.repository = new FolderRepository();
            //this.documentRepository = new DocumentRepository();
            //temp
            //this.publicRepository = new PublicRepository();
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

            System.Web.HttpContext.Current.Session["Role"] = Role; //tempory until I set up a real role checker

            try
            {
                TempData["Client_Name"] = folder.Name;
                TempData["Client_Id"] = folder.Number;
            }
            catch {
                return HttpNotFound();
            }

            return RedirectToAction("Index", "PublicVM", new { publicId = folder.Folder_ID });
        }

        //[HttpGet]
        ////action should eventually be moved into PublicVMController for a role specific button to address
        ////Downloads all documents pertaining to a specific client ID
        //public ActionResult DownloadAllDocuments([Bind(Prefix = "ClientId")] string Number) {

        //    tbl_Folder folder = repository.SelectByNumber(Number);
            
        //    //gets all documents for one folder_id
        //    IEnumerable<tbl_Document> files = documentRepository.SelectAll(folder.Folder_ID.ToString());

        //    byte[] result; //blank byte array, ready to be used to filled with the tbl_document.ArchivedFile bytes

        //    //opening a stream to allow data to be moved
        //    using (var zipArchiveMemoryStream = new MemoryStream()) {

        //        //creating a zipArchive obj to be used and disposed of
        //        using (var zipArchive = new ZipArchive(zipArchiveMemoryStream, ZipArchiveMode.Create, true)) {

        //            foreach (var file in files) {
        //                if (file.ArchivedFile != null) {
        //                    //according to Ramin, creation of an ArchivedFile and Submitting an ArchivedFile are different steps, so there could be 'dirty' records/documents in WAS db that has no ArchivedFile Fields records
        //                    var zipEntry = zipArchive.CreateEntry(file.Document_ID.ToString() + "." + file.FileExtension); //creates a unit of space for the individual file to be placed in

        //                    using (var entryStream = zipEntry.Open()) {
        //                        using (var tmpMemory = new MemoryStream(file.ArchivedFile))
        //                        {
        //                            tmpMemory.CopyTo(entryStream); //copies the data into the unit space
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        zipArchiveMemoryStream.Seek(0, SeekOrigin.Begin); //not exactly sure what this does, I think it pertains to the ziped folders ordering
        //        result = zipArchiveMemoryStream.ToArray(); //I think this is the ziped item
        //    }

        //    return new FileContentResult(result, "application/zip") { FileDownloadName = "AllArchivedFilesFor_" + Number + ".zip" };
        //}

        //Dispose any open connection when finished (db in this regard)
        protected override void Dispose(bool disposing)
        {
            repository.Dispose();

            base.Dispose(disposing);
        }
    }
}