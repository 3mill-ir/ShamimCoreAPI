using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class PollAnswerDataModel
    {
        
        public int ID { get; set; }
        public string Text { get; set; }
        public Nullable<int> AnswerKey { get; set; }       
        public Nullable<int> Score { get; set; }
        public Nullable<int> F_PollQuestionID { get; set; }
        public string Color { get; set; }
      public string IPAddress { get; set; }
         public string Device { get; set; }
        public virtual ICollection<PollLogDataModel> PollLog { get; set; }
    }

    public class PollAnswerTextDataModel
    {
        public List<PollAnswerDataModel> PollAnswerChoices { get; set; }
        public PollAnswerTextDataModel() {
             PollAnswerChoices = new List<PollAnswerDataModel>();
        }
    }
}