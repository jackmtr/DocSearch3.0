﻿using DocSearch2._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocSearch2._1.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private WASEntities _db = null;

        public FolderRepository()
        {
            this._db = new WASEntities();
        }

        public tbl_Folder SelectByID(string id)
        {
            //do this better
            return _db.tbl_Folder.Find(Int32.Parse(id));
        }

        public tbl_Folder SelectByNumber(string number) {

            int clientId = Int32.Parse(number);

            return _db.tbl_Folder.SingleOrDefault(folder => folder.Number == clientId);
        }

        public void Dispose() {
            _db.Dispose();

        }
    }
}