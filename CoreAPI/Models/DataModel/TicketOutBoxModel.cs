using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TicketOutBoxModel
    {
        public int ID { get; set; }
        public string Content_One { get; set; }
        public bool isRead { get; set; }
        public string SMSText { get; set; }
        public string SMSStatusOne { get; set; }
        public string SMSStatusTwo { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public string CreatedOnUTCJalali { get; set; }
        public int F_TicketInbox_Id { get; set; }
        public string UserRole { get; set; }
    }
}