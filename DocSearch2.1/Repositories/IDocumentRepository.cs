using DocSearch2._1.Models;
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
        tbl_Document SelectById(string id);

        IEnumerable<MiscPublicData> GetMiscPublicData(string publicNumber);

        void Dispose();
    }
}
