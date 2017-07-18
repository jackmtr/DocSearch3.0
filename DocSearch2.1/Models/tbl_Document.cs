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
    
    public partial class tbl_Document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_Document()
        {
            this.tbl_DocReference = new HashSet<tbl_DocReference>();
            this.tbl_Updates = new HashSet<tbl_Updates>();
        }
    
        public int Document_ID { get; set; }
        public int Folder_ID { get; set; }
        public int Repository_ID { get; set; }
        public int DocumentType_ID { get; set; }
        public string DocumentNumber { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Issue_DT { get; set; }
        public Nullable<System.DateTime> Received_DT { get; set; }
        public string Division_CD { get; set; }
        public string Direction { get; set; }
        public string Reason { get; set; }
        public string Method { get; set; }
        public string Recipient { get; set; }
        public string Originator { get; set; }
        public int Security { get; set; }
        public string Filepath { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public byte WASDocFlag { get; set; }
        public string FileDestination { get; set; }
        public byte[] ArchivedFile { get; set; }
        public string ArchivedXML { get; set; }
        public System.DateTime LastUser_DT { get; set; }
        public bool Active_IND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_DocReference> tbl_DocReference { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Updates> tbl_Updates { get; set; }
    }
}
