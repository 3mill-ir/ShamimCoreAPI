using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TicketInboxMediaModel
    {
        public int ID { get; set; }
        public string MediaPath { get; set; }
        public string MediaType { get; set; }
        public int F_TicketInbox_Id { get; set; }
    }
}