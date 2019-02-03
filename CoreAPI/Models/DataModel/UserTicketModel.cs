using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class UserTicketModel
    {
        public UserTicketModel()
        {
            MediaBox = new List<Media>();
        }
        public string Content { get; set; }
        public List<Media> MediaBox { get; set; }
        public int ID { get; set; }
        public int TrackingTicketID { get; set; }
        public string TrackingCode { get; set; }
        public string Tell { get; set; }
        public string profile { get; set; }
    }

    public class Media
    {
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Tell { get; set; }
    }
}