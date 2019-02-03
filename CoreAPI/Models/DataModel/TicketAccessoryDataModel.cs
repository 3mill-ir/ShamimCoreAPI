using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CoreAPI.Models.DataModel
{
    public class TicketAccessoryDataModel
    {
        public int UnResponseTicketCount { get; set; }
        public int ToResponseTicketCount { get; set; }
        public int ResponseTicketCount { get; set; }
        public int AllTicketCount { get; set; }
    }
}