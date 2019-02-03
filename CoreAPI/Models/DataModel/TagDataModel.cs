using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TagDataModel
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string F_UserID { get; set; }
    }
    public class TagDataListModel
    {
        public List<TagDataModel> ListTag { get; set; }
        public TagDataListModel()
        {
            ListTag = new List<TagDataModel>();
        }
    }
}