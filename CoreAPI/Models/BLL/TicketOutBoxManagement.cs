using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoreAPI.Models.BLL
{
    public class TicketOutBoxManagement
    {
        private Entities db { get; set; }
        public TicketOutBoxManagement(Entities _db)
        {
            db = _db;
        }
        public int AddTicketOutBox(TicketOutBoxModel model, string F_UserId)
        {
            var ListObject = db.TicketInbox.Include(m => m.Ticket).FirstOrDefault(m => m.ID == model.F_TicketInbox_Id && m.Ticket.F_UserID == F_UserId);
            if (ListObject != null)
            {
                ListObject.Ticket.Status = "پاسخ داده شده";
                db.SaveChanges();
            }
            TicketOutBox InsertObject = new TicketOutBox();
            InsertObject.CreatedOnUTC = DateTime.Now;
            InsertObject.Content_One = model.Content_One;
            InsertObject.F_TicketInboxID = model.F_TicketInbox_Id;
            InsertObject.IsRead = false;
            db.TicketOutBox.Add(InsertObject);
            db.SaveChanges();
            return 1;
        }

        public List<TicketOutBoxModel> ListTicketOutBox(int F_TicketInbox_Id)
        {
            var ListObject = db.TicketOutBox.Where(m => m.F_TicketInboxID == F_TicketInbox_Id);
            List<TicketOutBoxModel> list = new List<TicketOutBoxModel>();
            foreach (var ListItem in ListObject)
            {
                TicketOutBoxModel t = new TicketOutBoxModel();
                t.CreatedOnUTC = ListItem.CreatedOnUTC ?? default(DateTime);
                t.Content_One = ListItem.Content_One;
                t.F_TicketInbox_Id = ListItem.F_TicketInboxID ?? default(int);
                t.isRead = ListItem.IsRead ?? default(bool);
                t.SMSStatusOne = ListItem.SMSStatusOne;
                t.SMSStatusTwo = ListItem.SMSStatusTwo;
                t.SMSText = ListItem.SMSText;
                t.ID = ListItem.ID;
                list.Add(t);
            }
            return list;
        }
    }
}