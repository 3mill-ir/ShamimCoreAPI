using AutoMapper.XpressionMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TicketModel
    {
        public int ID { get; set; }
        public string TrackingCode { get; set; }
        public string ID_STR { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdateOnUtc { get; set; }
        public string LastUpdateOnUtcJalali { get; set; }
        public string TicketInbox_brief { get; set; }
        public int CountInbox { get; set; }
        public int CountOutbox { get; set; }
        public int CountInboxMedia { get; set; }



        public Collection<TicketInboxModel> TicketInbox { get; set; }
    }
}