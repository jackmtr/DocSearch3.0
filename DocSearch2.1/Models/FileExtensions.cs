﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocSearch2._1.Models
{
    //not even using this enum
    public enum FileExtensions
    {
        pdf = 1, gif, jpg, msg, ppt, xls, csv, xlsx, doc, dot, docx, html
    }

    public enum NavBarGroupOptions
    {
        category, doctype, policy
    }
}