using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocSearch2._1.Models;

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
        public void Dispose()
        {
            _db.Dispose();
        }

    }
}