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
    
    public partial class AttributeItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AttributeItem()
        {
            this.Attribute = new HashSet<Attribute>();
            this.AttributeValue = new HashSet<AttributeValue>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> F_AttributeID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attribute> Attribute { get; set; }
        public virtual Attribute Attribute1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttributeValue> AttributeValue { get; set; }
    }
}
