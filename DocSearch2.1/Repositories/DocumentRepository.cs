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

        public IEnumerable<MiscPublicData> GetMiscPublicData(string id)
        {
            //should be able to create a function to do this repetitive code from both functions
            List<MiscPublicData> MiscPublicList = new List<MiscPublicData>();

            int documentNumberInt = Int32.Parse(id);

            var documentList = (from d in _db.tbl_Document
                                where d.Document_ID == documentNumberInt
                                select new
                                {
                                    d.Division_CD,
                                    d.CreatorFirstName,
                                    d.CreatorLastName,
                                    d.LastUser_DT
                                }).ToList();

            foreach (var item in documentList)
            {
                MiscPublicData mpd = new MiscPublicData();

                mpd.Branch = item.Division_CD;
                mpd.Creator = item.CreatorFirstName + " " + item.CreatorLastName;
                mpd.ArchiveTime = item.LastUser_DT;

                MiscPublicList.Add(mpd);
            }

            return MiscPublicList;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}