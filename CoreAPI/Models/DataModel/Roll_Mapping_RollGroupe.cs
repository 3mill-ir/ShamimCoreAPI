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
    
    public partial class Roll_Mapping_RollGroupe
    {
        public int ID { get; set; }
        public string F_RollGroupeID { get; set; }
        public string F_RollID { get; set; }
    
        public virtual AspNetRoles AspNetRoles { get; set; }
        public virtual RollGroupe RollGroupe { get; set; }
    }
}