using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TicketInboxModel
    {
        public TicketInboxModel()
        {
            TicketOutbox = new List<TicketOutBoxModel>();
            TicketInboxMedia = new List<TicketInboxMediaModel>();
        }
        public int ID { get; set; }
        public string TicketContent { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public string CreatedOnUTCJalali { get; set; }
        public string TicketType { get; set; }
        public string TicketFrom { get; set; }
        public int F_Ticket_Id { get; set; }


        public List<TicketOutBoxModel> TicketOutbox { get; set; }

        public List<TicketInboxMediaModel> TicketInboxMedia { get; set; }
    }
}