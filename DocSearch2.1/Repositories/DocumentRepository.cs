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
            return _db.tbl_Document.Find(Int32.Parse(id));
        }

        public MiscPublicData GetMiscPublicData(string id)
        {
            //should be able to create a function to do this repetitive code from both functions

            int documentNumberInt = Int32.Parse(id);

            var documentData = (from d in _db.tbl_Document
                                where d.Document_ID == documentNumberInt
                                select new
                                {
                                    d.Document_ID,
                                    d.Division_CD,
                                    d.CreatorFirstName,
                                    d.CreatorLastName,
                                    d.LastUser_DT
                                }).Single();


            MiscPublicData mpd = new MiscPublicData();

            mpd.Document_ID = documentData.Document_ID;
            mpd.Branch = documentData.Division_CD;
            mpd.Creator = documentData.CreatorFirstName + " " + documentData.CreatorLastName;
            mpd.ArchiveTime = documentData.LastUser_DT;

            return mpd;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}