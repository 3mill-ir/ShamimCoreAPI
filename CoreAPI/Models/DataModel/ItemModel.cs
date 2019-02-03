using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class ItemModel
    {
        public int ID { get; set; }
        public int F_MenuID { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public int Vote { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string Functionality { get; set; }
        public string Url { get; set; }
       
    }
    public class NewsItemModel
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string Content { get; set; }
        public string Functionality { get; set; }
        public string Url { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comment { get; set; }
    }

}