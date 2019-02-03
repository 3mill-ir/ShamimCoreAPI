using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoreAPI.Models.BLL
{
    public class TicketInboxManagement
    {
        private Entities db { get; set; }
        public TicketInboxManagement(Entities _db)
        {
            db = _db;
        }
        public int AddTicketInbox(UserTicketModel model, string profile)
        {
            TicketInbox InsertObject = new TicketInbox();
            InsertObject.CreatedOnUTC = DateTime.Now;
            InsertObject.F_TicketID = model.ID;
            InsertObject.TicketFrom = "Web";
            InsertObject.TicketContent = model.Content;
            string TiketType = "";
            TiketType += "Text";
            if (model.MediaBox.Count != 0)
                TiketType = "Multimedia";
            InsertObject.TicketType = TiketType;
            db.TicketInbox.Add(InsertObject);
            db.SaveChanges();
            TicketLogManagement TLM = new TicketLogManagement(new Entities());
            TLM.AddTicketLog(InsertObject.ID);
            //TicketInboxMediaManagement TIMM = new TicketInboxMediaManagement(new Entities());
            //TIMM.AddTicketInboxMedia(model, InsertObject.ID, profile);
            return 1;
        }

        public int SMSAddTicketInbox(UserTicketModel model)
        {
            TicketInbox InsertObject = new TicketInbox();
            InsertObject.CreatedOnUTC = DateTime.Now;
            InsertObject.F_TicketID = model.ID;
            InsertObject.TicketFrom = "SMS";
            InsertObject.TicketContent = model.Content;
            string TiketType = "Text";
            InsertObject.TicketType = TiketType;
            db.TicketInbox.Add(InsertObject);
            db.SaveChanges();
            TicketLogManagement TLM = new TicketLogManagement(new Entities());
            TLM.SMSAddTicketLog(InsertObject.ID, model.Tell);
            return 1;
        }
        public List<TicketInboxModel> ListTicketInbox(string F_UserID, int F_Ticket_Id)
        {
            var ListObject = db.TicketInbox.Include(m => m.Ticket).Where(m => m.F_TicketID == F_Ticket_Id && m.Ticket.F_UserID == F_UserID);
            List<TicketInboxModel> list = new List<TicketInboxModel>();
            foreach (var ListItem in ListObject)
            {
                TicketInboxModel t = new TicketInboxModel();
                t.CreatedOnUTC = ListItem.CreatedOnUTC ?? default(DateTime);
                t.F_Ticket_Id = ListItem.F_TicketID ?? default(int);
                t.TicketFrom = ListItem.TicketFrom ?? default(string);
                t.TicketContent = ListItem.TicketContent;
                t.TicketType = ListItem.TicketType;
                t.ID = ListItem.ID;
                TicketOutBoxManagement Outbox = new TicketOutBoxManagement(db);
                t.TicketOutbox = Outbox.ListTicketOutBox(ListItem.ID).OrderByDescending(u => u.CreatedOnUTC).ToList();
                list.Add(t);
            }
            return list;
        }
        public List<TicketInboxModel> ListAllTicketInboxes(string profile)
        {
            string F_UserID = Tools.UserID(profile);
            var ListObject = db.TicketInbox.Where(m => m.Ticket.F_UserID == F_UserID).OrderByDescending(u => u.CreatedOnUTC).Take(10);
            List<TicketInboxModel> list = new List<TicketInboxModel>();
            foreach (var ListItem in ListObject)
            {
                TicketInboxModel t = new TicketInboxModel();
                t.TicketContent = ListItem.TicketContent;
                t.TicketFrom = ListItem.TicketFrom;
                t.ID = ListItem.ID;
                list.Add(t);
            }
            return list;
        }
    }
}