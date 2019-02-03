using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class AttributeValueDataModel
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public Nullable<int> F_AttributeID { get; set; }
        public Nullable<int> F_AttributeItemID { get; set; }
        public Nullable<int> F_ItemID { get; set; }
    }
}