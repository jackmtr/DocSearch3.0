//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DocSearch2._1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_DocumentType
    {
        public int DocumentType_ID { get; set; }
        public int Cabinet_ID { get; set; }
        public int Category_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active_IND { get; set; }
        public int Security { get; set; }
        public int LastUser_ID { get; set; }
        public System.DateTime LastUser_DT { get; set; }
    
        public virtual tbl_Cabinet tbl_Cabinet { get; set; }
        public virtual tbl_Category tbl_Category { get; set; }
        public virtual tbl_User tbl_User { get; set; }
    }
}
