using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoreAPI.Models.DataModel
{

    public class ListPostDataModel
    {

        public ListPostDataModel()
        {
            List = new List<PostDataModel>();
        }
        public int count { get; set; }
        public List<PostDataModel> List { get; set; }
    }
        public class PostDataModel
    {
        public int ID { get; set; }
        public string Tittle { get; set;}
        public string Description { get; set; }

  
        public string Detail { get; set; }
        public  string ImagePath { get; set; }
        public Nullable<bool> isDeleted {get; set;}
        public Nullable<bool> Status { get; set; }
        public string F_UserID { get; set; }
        public Nullable<int> F_MenuID { get; set; }
        public Nullable<int> NumberOfVisitors { get; set; }
        public Nullable<int> NumberOfComments { get; set; }
        public Nullable<int> NumberOfLikes { get; set; }
        public Nullable<int> NumberOfDislikes { get; set; }
        public Nullable<System.DateTime> CreatedOnUTC { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTittle { get;set;}
        public string MetaSeoName { get; set; }
        public string ImageAlt { get; set; }
        public bool AllowComment { get; set; }
        public string Language { get; set; }

        public string MenuName { get; set; }

        public  ICollection<CommentDataModel> Comments1 { get; set; }
        
    }
}