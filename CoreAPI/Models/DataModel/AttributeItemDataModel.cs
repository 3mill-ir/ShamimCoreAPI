using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class AttributeItemDataModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> F_AttributeID { get; set; }
      //  public string Value { get; set; }
        public ICollection<AttributeDataModel> Attribute { get; set; }
    }
}