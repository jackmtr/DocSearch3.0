﻿using DocSearch2._1.Models;
using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSearch2._1.Repositories
{
    public interface IDocumentRepository
    {
        tbl_Document SelectById(string id, bool authorized);

        IEnumerable<tbl_Document> SelectAll(string id);

        MiscPublicData GetMiscPublicData(string publicNumber);

        bool SaveChanges(tbl_Document doc);

        void Update(tbl_Document doc);

        void Save();

        void Dispose();
    }
}
