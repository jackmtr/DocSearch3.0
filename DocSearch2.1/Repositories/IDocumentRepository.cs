using DocSearch2._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSearch2._1.Repositories
{
    public interface IDocumentRepository
    {
        tbl_Document SelectById(string id);

        void Dispose();
    }
}
