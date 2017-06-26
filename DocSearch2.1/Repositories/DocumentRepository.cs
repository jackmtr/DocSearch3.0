using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocSearch2._1.Models;
using DocSearch2._1.ViewModels;

namespace DocSearch2._1.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private WASEntities _db = null;

        public DocumentRepository()
        {
            this._db = new WASEntities();
        }

        public tbl_Document SelectById(string id)
        {
            tbl_Document document = _db.tbl_Document.Find(Int32.Parse(id));

            //if document exists and ArchiveFile is null, it will look into the purged WAS db instead.
            if (document.ArchivedFile == null) {
                this._db = new WASEntities("name=WASArchiveEntities");
                document = _db.tbl_Document.Find(Int32.Parse(id));

                //Because this is a rare occurance, I would rather blindly search through other db's than change my model to bring in the repo value
            }

            return document;
        }

        public IEnumerable<tbl_Document> SelectAll(string id) {

            int intId = Int32.Parse(id);

            return _db.tbl_Document.AsNoTracking().Where(d => d.Folder_ID == intId);

            //return documents;
        }

        public MiscPublicData GetMiscPublicData(string id)
        {
            //should be able to create a function to do this repetitive code from both functions

            int documentNumberInt = Int32.Parse(id);

            var documentData = (from d in _db.tbl_Document.AsNoTracking() //.AsNoTracking reduces resources by making this read only      
                                //not every document will have a corrosponding docReference
                                join dr in _db.tbl_DocReference on d.Document_ID equals dr.Document_ID into ps
                                where d.Document_ID == documentNumberInt
                                from dr in ps.DefaultIfEmpty()
                                select new
                                {
                                    d.Document_ID,
                                    d.Division_CD,
                                    d.CreatorFirstName,
                                    d.CreatorLastName,
                                    d.LastUser_DT,
                                    d.Reason,
                                    d.Recipient,
                                    d.tbl_DocReference
                                }).First();
            //instead of doing .First(), should be a better way of bringing over just 1 record since they SHOULD(?) all be the same, probably a better LINQ statement
            //torn between making a subclass for docReference to pull those 3 properties from, or just use the full dataset.  

            MiscPublicData mpd = new MiscPublicData();

            mpd.Document_ID = documentData.Document_ID;
            mpd.Branch = documentData.Division_CD;
            mpd.Creator = documentData.CreatorFirstName + " " + documentData.CreatorLastName;
            mpd.ArchiveTime = documentData.LastUser_DT;
            mpd.Reason = documentData.Reason;
            mpd.Recipient = documentData.Recipient;
            mpd.DocReferences = documentData.tbl_DocReference;

            return mpd;
        }

        public bool SaveChanges(tbl_Document doc) {

            //Now, i'm using AutoMapper. So i thought i could map from the ViewModel to the Post, then save the post.

            return true;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}