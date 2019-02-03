using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class CommentDataModel
    {
      
        public int ID { get; set; }
        
        public string Text { get; set; }
       
        public DateTime CreatedDateOnUTC { get; set; }
        
        public bool Dispaly { get; set; }
     
        public Nullable<int> NumberOfReply { get; set; }
        
        public Nullable<int> NumberOfLikes { get; set; }
        
        public Nullable<int> NumberOfDislikes { get; set; }
         
        public Nullable<int> F_PostsID { get; set; }
     
        public Nullable<int> F_CommentsID { get; set; }
      
        public string F_UserID { get; set; }
     
        public string IPAddress { get; set; }

        public  ICollection<CommentDataModel> Comments1 { get; set; }
      
    }
}