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
    
    public partial class ComponentSetUp
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ComponentSetUp()
        {
            this.ComponentItemSetUp = new HashSet<ComponentItemSetUp>();
        }
    
        public int ID { get; set; }
        public string Component { get; set; }
        public Nullable<double> Weight { get; set; }
        public string F_UserID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComponentItemSetUp> ComponentItemSetUp { get; set; }
    }
}
