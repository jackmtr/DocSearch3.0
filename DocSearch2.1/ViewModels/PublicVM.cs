﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocSearch2._1.ViewModels
{
    public class PublicVM
    {
        public int Folder_ID { get; set; } //tbl_Folder.Folder_ID

        public int Document_ID { get; set; } //tbl_Document.Document_ID

        public int DocumentType_ID { get; set; } //tbl_DocumentType.DocumentType_ID

        [Display(Name = "Type of Document")]
        public string DocumentTypeName { get; set; } //tbl_DocumentType.Name

        [Display(Name = "Issue Date")]
        public Nullable<System.DateTime> IssueDate { get; set; } //tbl_Document.Issue_DT

        public string Description { get; set; } //tbl_Document.Description

        public int Category_ID { get; set; } //tbl_Category.Category_ID

        [Display(Name = "Category")]
        public string CategoryName { get; set; } //tbl_Category.Name

        [Display(Name = "Effective Date")]
        public Nullable<System.DateTime> EffectiveDate { get; set; } //tbl_DocReference.Date1_DT

        public string RefNumber { get; set; } //tbl_DocReference.RefNumber

        //file stuff properties need to be added
    }
}