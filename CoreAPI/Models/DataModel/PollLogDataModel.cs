using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class PollLogDataModel
    {
        public int ID { get; set; }
        public string IPAddress { get; set; }
        public string F_UserID { get; set; }
        public string Device { get; set; }
        public Nullable<System.DateTime> CreatedOnUTC { get; set; }
        public Nullable<int> F_PollAnswerID { get; set; }
    }
}