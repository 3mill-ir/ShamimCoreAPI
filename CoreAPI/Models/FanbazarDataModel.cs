using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models
{
    public class FanbazarDataModel
    {
        public ICollection<AttributeGroupDataModel> AttributeGoups { get; set; }
        public ItemDataModel Item { get; set; }
    }

    public class ItemPostDataModel
    {
        public ItemPostDataModel()
        {
            Attributes = new List<ItemPostHelper>();
        }

        public int ID { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int F_MenuID { get; set; }
        public List<ItemPostHelper> Attributes { get; set; }
    }
    public class ItemPostHelper
    {
        public int F_AttributeID { get; set; }
        public string Value { get; set; }
        public int F_AttributeItemID { get; set; }
    }
}