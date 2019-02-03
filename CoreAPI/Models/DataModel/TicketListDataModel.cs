using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class TicketListDataModel
    {
        public TicketListDataModel()
        {
            Tickets = new List<TicketModel>();
        }
        public List<TicketModel> Tickets { get; set; }
        public int Total { get; set; }
    }
}