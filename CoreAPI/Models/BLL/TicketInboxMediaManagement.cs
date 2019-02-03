using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.BLL
{
    public class TicketInboxMediaManagement
    {
        private Entities db { get; set; }
        public TicketInboxMediaManagement(Entities _db)
        {
            db = _db;
        }
        public int AddTicketInboxMedia(UserTicketModel model, int F_TicketInbox_Id, string prfoile)
        {
            foreach (var item in model.MediaBox)
            {
                TicketInboxMedia InsertObject = new TicketInboxMedia();
                InsertObject.F_TicketInboxID = F_TicketInbox_Id;
                InsertObject.MediaType = item.ContentType;
                InsertObject.MediaPath = item.FilePath;
                db.TicketInboxMedia.Add(InsertObject);
            }
            db.SaveChanges();
            return 1;
        }
        public List<TicketInboxMediaModel> ListTicketInboxMedia(int F_TicketInbox_Id)
        {
            var ListObject = db.TicketInboxMedia.Where(m => m.F_TicketInboxID == F_TicketInbox_Id);
            List<TicketInboxMediaModel> list = new List<TicketInboxMediaModel>();
            foreach (var ListItem in ListObject)
            {
                TicketInboxMediaModel t = new TicketInboxMediaModel();
                t.F_TicketInbox_Id = ListItem.F_TicketInboxID ?? default(int);
                t.MediaPath = ListItem.MediaPath;
                t.MediaType = ListItem.MediaType;
                list.Add(t);
            }
            return list;
        }
    }
}