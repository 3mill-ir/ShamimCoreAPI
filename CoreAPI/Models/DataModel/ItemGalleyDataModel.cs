using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{

    public class ItemGalleyListDataModel
    {
        public int F_ItemID { get; set; }
        public List< ItemGalleyDataModel> itemGallery { get; set; }

    }
    public class ItemGalleyDataModel
    {
        public Nullable<int> ID { get; set; }
        public string FileName { get; set; }
    }
}