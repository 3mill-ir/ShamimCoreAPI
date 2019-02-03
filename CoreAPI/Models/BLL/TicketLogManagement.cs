using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoreAPI.Models.DataModel;

namespace CoreAPI.Models.BLL
{
    public class TicketLogManagement
    {
        private Entities db { get; set; }
        public TicketLogManagement(Entities _db)
        {
            db = _db;
        }
        public int AddTicketLog(int F_TicketInbox_Id)
        {
            TicketLog InsertObject = new TicketLog();
            InsertObject.CreatedOnUTC = DateTime.Now;
            string IP = HttpContext.Current.Request.UserHostAddress;
            if (IP == "::1")
                IP = "127.0.0.1";
            InsertObject.Address = IP;
            InsertObject.Device = "Web";
            InsertObject.F_TicketInboxID = F_TicketInbox_Id;
            db.TicketLog.Add(InsertObject);
            db.SaveChanges();
            return 1;
        }

        public int AndroidAddTicketLog(int F_TicketInbox_Id)
        {
            TicketLog InsertObject = new TicketLog();
            InsertObject.CreatedOnUTC = DateTime.Now;
            string IP = HttpContext.Current.Request.UserHostAddress;
            if (IP == "::1")
                IP = "127.0.0.1";
            InsertObject.Address = IP;
            InsertObject.Device = "Android";
            InsertObject.F_TicketInboxID = F_TicketInbox_Id;
            db.TicketLog.Add(InsertObject);
            db.SaveChanges();
            return 1;
        }

        public int SMSAddTicketLog(int F_TicketInbox_Id, string Tell)
        {
            TicketLog InsertObject = new TicketLog();
            InsertObject.CreatedOnUTC = DateTime.Now;
            InsertObject.Address = Tell;
            InsertObject.Device = "SMS";
            InsertObject.F_TicketInboxID = F_TicketInbox_Id;
            db.TicketLog.Add(InsertObject);
            db.SaveChanges();
            return 1;
        }

    }
}