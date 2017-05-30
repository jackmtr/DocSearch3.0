using DocSearch2._1.Models;
using DocSearch2._1.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private IDocumentRepository documentRepository = null;
        private static string directoryPath = @"C:\Users\jcheng\Downloads";

        public FolderController() {
            this.repository = new FolderRepository();
            this.documentRepository = new DocumentRepository();
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
            return RedirectToAction("Index", "PublicVM", new { publicId = folder.Folder_ID });
        }

        public ActionResult DownloadAllDocuments([Bind(Prefix = "ClientId")] string Number) {

            tbl_Folder folder = repository.SelectByNumber(Number);

            IEnumerable<tbl_Document> files = documentRepository.SelectAll(folder.Folder_ID.ToString());

            //string MimeType = null;

            byte[] result;

            using (var zipArchiveMemoryStream = new MemoryStream()) {

                using (var zipArchive = new ZipArchive(zipArchiveMemoryStream, ZipArchiveMode.Create, true)) {

                    foreach (var file in files) {

                        var zipEntry = zipArchive.CreateEntry(file.Document_ID.ToString() + "." + file.FileExtension);

                        //switch (file.FileExtension.ToLower().Trim())
                        //{
                        //    case "pdf":
                        //        MimeType = "application/pdf";
                        //        break;

                        //    case "gif":
                        //        MimeType = "image/gif";
                        //        break;

                        //    case "jpg":
                        //        MimeType = "image/jpeg";
                        //        break;

                        //    case "msg":
                        //        MimeType = "application/vnd.outlook";
                        //        break;

                        //    case "ppt":
                        //        MimeType = "application/vnd.ms-powerpoint";
                        //        break;

                        //    case "xls":
                        //    case "csv":
                        //        MimeType = "application/vnd.ms-excel";
                        //        break;

                        //    case "xlsx":
                        //        MimeType = "application/vnd.ms-excel.12";
                        //        break;

                        //    case "doc":
                        //    case "dot":
                        //        MimeType = "application/msword";
                        //        break;

                        //    case "docx":
                        //        MimeType = "application/vnd.ms-word.document.12";
                        //        break;

                        //    default:
                        //        MimeType = "text/html";
                        //        break;
                        //}

                        using (var entryStream = zipEntry.Open()) {
                            using (var tmpMemory = new MemoryStream(file.ArchivedFile))
                            {
                                tmpMemory.CopyTo(entryStream);
                            }
                        }
                    }
                }

                zipArchiveMemoryStream.Seek(0, SeekOrigin.Begin);
                result = zipArchiveMemoryStream.ToArray();
            }

            return new FileContentResult(result, "application/zip") { FileDownloadName = Number + ".zip" };
        }

        //Dispose any open connection when finished (db in this regard)
        protected override void Dispose(bool disposing)
        {
            repository.Dispose();

            base.Dispose(disposing);
        }
    }
}