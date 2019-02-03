using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TagsPostsMappingDataModel
    {
        public int ID { get; set; }
        public int F_TagsID { get; set; }
        public int F_PostsID { get; set; }
    }
}