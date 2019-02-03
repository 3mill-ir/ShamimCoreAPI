using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class ComponentSetUpDataModel
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public Nullable<double> Weight { get; set; }
        public string F_UserID { get; set; }
        public ICollection<ComponentItemSetUpDataModel> ComponentItemSetUp { get; set; }
    }
}