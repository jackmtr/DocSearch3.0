using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocSearch2._1.ViewModels
{
    public class MiscPublicData
    {
        public string Branch { get; set; }
        public string Creator { get; set; }

        [Display(Name = "Archive Time")]
        //[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public Nullable<DateTime> ArchiveTime { get; set; }

    }
}