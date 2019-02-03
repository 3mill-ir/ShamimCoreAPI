using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class ComponentItemSetUpDataModel
    {
        public int ID { get; set; }
        public Nullable<int> F_ComponentID { get; set; }
        public Nullable<int> F_ItemID { get; set; }
        public string Type { get; set; }
        public Nullable<double> Weight { get; set; }
    }
}