using DocSearch2._1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSearch2._1.Repositories
{
    public interface IPublicRepository
    {
        IEnumerable<PublicVM> SelectAll(string publicNumber);
    }
}
