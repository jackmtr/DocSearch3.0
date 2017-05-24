using DocSearch2._1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocSearch2._1.ViewModels
{
    public class MiscPublicData
    {
        public int Document_ID { get; set; }

        public string Branch { get; set; }

        public string Creator { get; set; }

        [Display(Name = "Archive Time")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> ArchiveTime { get; set; }

        public string Reason { get; set; }

        public string Recipient { get; set; }

        public ICollection<tbl_DocReference> DocReferences { get; set; }

    }
}