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
    
    public partial class ShekayatInboxOutBox
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> CreatedDateOnUtc { get; set; }
        public Nullable<int> F_ShekayatID { get; set; }
    
        public virtual Shekayat Shekayat { get; set; }
    }
}
