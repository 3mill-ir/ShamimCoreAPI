//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoreAPI.Models.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shekayat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Shekayat()
        {
            this.ShekayatInboxOutBox = new HashSet<ShekayatInboxOutBox>();
        }
    
        public int ID { get; set; }
        public string ShakiFirstName { get; set; }
        public string ShakiLastName { get; set; }
        public string MotashakiFirstName { get; set; }
        public string MotashakiLastName { get; set; }
        public string ShakiSemat { get; set; }
        public string MotashakiSemat { get; set; }
        public string ShakiCodeMelli { get; set; }
        public string MotashakiOstan { get; set; }
        public string NoeErtebat { get; set; }
        public string MotashakiHozeKhedmat { get; set; }
        public string ShakiTell { get; set; }
        public string ShakiMobile { get; set; }
        public string ShakiAddress { get; set; }
        public Nullable<System.DateTime> TarikheName { get; set; }
        public string ShomareNaame { get; set; }
        public string NaameMahaleMorajee { get; set; }
        public string NatijeEghdameGhabli { get; set; }
        public Nullable<System.DateTime> LastUpdatedOnUTC { get; set; }
        public string Subject { get; set; }
        public string F_UserID { get; set; }
        public string TrackingCode { get; set; }
        public string Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShekayatInboxOutBox> ShekayatInboxOutBox { get; set; }
    }
}
