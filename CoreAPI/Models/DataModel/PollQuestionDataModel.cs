using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class PollQuestionDataModel
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public Nullable<DateTime> CreatedOnUTC { get; set; }
        public Nullable<DateTime> StartDateOnUTC { get; set; }
        public Nullable<DateTime> EndDateOnUTC { get; set; }
        public string F_UserID { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public string ChartType { get; set; }
        public  List<PollAnswerDataModel> PollAnswer { get; set; }

        public bool Active { get; set; }

    }
}