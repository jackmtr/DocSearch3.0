using DocSearch2._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSearch2._1.Repositories
{
    public interface IFolderRepository
    {
        tbl_Folder SelectByID(string id);

        void Dispose();
    }
}
