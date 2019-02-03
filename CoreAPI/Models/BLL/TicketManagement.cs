using CoreAPI.Models.DataModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoreAPI.Models.BLL
{
    public class TicketManagement
    {
        private Entities db { get; set; }
        public TicketManagement(string profile, Entities _db)
        {
            F_UserName = profile;
            Path = Tools.ReturnPathPhysicalMode("TicketChartPath", F_UserName, "AdminAddress", "TicketManagement()");
            db = _db;
        }
        string Path { get; set; }
        string F_UserName { get; set; }

        public string AddTicket(string profile, UserTicketModel model)
        {
            string result = "";
            if (model.ID == 0)
            {
                Ticket InsertObject = new Ticket();
                InsertObject.LastUpdatedOnUTC = DateTime.Now;
                InsertObject.Status = "در وضعیت انتظار";
                InsertObject.TrackingCode = UniqTrackingCodeGenerator();
                InsertObject.F_UserID = Tools.UserID(profile);
                db.Ticket.Add(InsertObject);
                db.SaveChanges();
                model.ID = InsertObject.ID;
                result = InsertObject.TrackingCode;
            }
            TicketInboxManagement TicketInbox = new TicketInboxManagement(db);
            TicketInbox.AddTicketInbox(model, profile);
            return result;
        }
        public int ChangeTicketStatus(TicketModel model, string F_UserID)
        {
            var EditObject = db.Ticket.FirstOrDefault(u => u.ID == model.ID && u.F_UserID == F_UserID);
            if (EditObject != null)
            {
                EditObject.Status = model.Status;
                db.SaveChanges();
                return 1;
            }
            else
                return 2;
        }

        public int DeleteTicket(Ticket model)
        {
            var DeleteObject = db.Ticket.FirstOrDefault(u => u.ID == model.ID);
            if (DeleteObject != null)
            {
                db.Ticket.Remove(DeleteObject);
                db.SaveChanges();
                return 1;
            }
            else
                return 2;
        }

        public List<TicketModel> ListTicket(string F_UserID, string SearchString, string TicketStatus, int pageNumber, int pageSize, out int total)
        {
            IPagedList<Ticket> ListObject;
            if (string.IsNullOrEmpty(TicketStatus))
            {
                if (string.IsNullOrEmpty(SearchString))
                {
                    ListObject = db.Ticket.Include(u=>u.TicketInbox).Where(u => u.F_UserID == F_UserID).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
                else
                {
                    ListObject = db.Ticket.Where(m => m.TrackingCode.ToLower().Contains(SearchString.ToLower()) && m.F_UserID == F_UserID).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SearchString))
                {
                    ListObject = db.Ticket.Where(m => m.Status == TicketStatus && m.F_UserID == F_UserID).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
                else
                {

                    ListObject = db.Ticket.Where(m => m.Status == TicketStatus && m.F_UserID == F_UserID).Where(m => m.TrackingCode.ToLower().Contains(SearchString.ToLower())).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
            }
            List<TicketModel> list = new List<TicketModel>();
            foreach (var ListItem in ListObject)
            {
                TicketModel t = new TicketModel();
                t.Status = ListItem.Status;
                t.LastUpdateOnUtc = ListItem.LastUpdatedOnUTC ?? default(DateTime);
                t.ID = ListItem.ID;
                t.ID_STR = ListItem.ID.ToString();
                t.TrackingCode = ListItem.TrackingCode;
                var inbox = ListItem.TicketInbox.OrderBy(m => m.CreatedOnUTC).FirstOrDefault();
                if (inbox != null)
                {
                    switch (inbox.TicketType)
                    {
                        case "Text":
                            if (!string.IsNullOrEmpty(inbox.TicketContent))
                                t.TicketInbox_brief = inbox.TicketContent;
                            break;
                        case "Voice":
                            t.TicketInbox_brief = "صوت";
                            break;
                        case "Video":
                            t.TicketInbox_brief = "ویدیو";
                            break;
                        case "Image":
                            t.TicketInbox_brief = "عکس";
                            break;
                        case "Document":
                            t.TicketInbox_brief = "سند";
                            break;
                        default:
                            if (!string.IsNullOrEmpty(inbox.TicketType) && inbox.TicketContent != null)
                                t.TicketInbox_brief = inbox.TicketContent;
                            break;
                    }
                    t.CountInbox = ListItem.TicketInbox.Count();
                    t.CountInboxMedia = ListItem.TicketInbox.Sum(m => m.TicketInboxMedia.Count());
                    t.CountOutbox = ListItem.TicketInbox.Sum(m => m.TicketOutBox.Count());
                }
                list.Add(t);
            }
            return list;
        }



        public TicketModel TicketBrief(int Id, string profile)
        {
            string F_UserId = Tools.UserID(profile);
            var Object = db.Ticket.FirstOrDefault(m => m.ID == Id && m.F_UserID == F_UserId);
            TicketModel t = new TicketModel();
            t.Status = Object.Status;
            t.LastUpdateOnUtc = Object.LastUpdatedOnUTC ?? default(DateTime);
            t.ID = Object.ID;
            t.TrackingCode = Object.TrackingCode;
            return t;
        }


        public string UniqTrackingCodeGenerator()
        {
            string result = "";
            int length = 8;
            bool loop = true;
            Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            do
            {
                string temp = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                var _object = db.Ticket.FirstOrDefault(u => u.TrackingCode == temp);
                if (_object == null)
                {
                    result = temp;
                    loop = false;
                }
                else
                {
                    loop = true;
                }
            } while (loop);
            return result;
        }




        #region Admin

        public static List<TicketTrackFactModel> TicketTrackFacts(string profile)
        {
            var db = new Entities();
            string F_userId = Tools.UserID(profile);
            TicketInboxManagement TIM = new TicketInboxManagement(db);
            var Statuses = (from c in db.Ticket where c.F_UserID == F_userId group c by c.Status into g select new { FieldCount = g.Count(), FieldName = g.Key }).ToList();
            List<TicketTrackFactModel> list = new List<TicketTrackFactModel>();
            foreach (var item in Statuses)
            {
                TicketTrackFactModel ListItem = new TicketTrackFactModel();
                ListItem.Count = item.FieldCount;
                ListItem.Name = item.FieldName;
                list.Add(ListItem);
            }
            return list;
        }
        #endregion

    }
}