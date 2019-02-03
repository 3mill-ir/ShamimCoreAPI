using CoreAPI.Models.DataModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AutoMapper;

namespace CoreAPI.Models.BLL
{
    public class ShekayatManagement
    {
        private Entities db { get; set; }

        public ShekayatManagement(Entities _db)
        {
            db = _db;
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Shekayat, ShekayatModel>();
                cfg.CreateMap<ShekayatModel, Shekayat>();
            });
        }
        string Path { get; set; }
        string F_UserName { get; set; }

        public string AddShekayat(string profile, ShekayatModel model)
        {
            string result = "";
            if (model.ID == 0)
            {
                Shekayat InsertObject = new Shekayat();
                model.LastUpdatedOnUTC = DateTime.Now;
                model.Status = "در وضعیت انتظار";
                model.TrackingCode = UniqTrackingCodeGenerator();
                model.F_UserID = Tools.UserID(profile);
                InsertObject = Mapper.Map<ShekayatModel, Shekayat>(model);
                db.Shekayat.Add(InsertObject);
                db.SaveChanges();
                model.ID = InsertObject.ID;
                model.F_ShekayatID = InsertObject.ID;
                result = InsertObject.TrackingCode;
            }
            ShekayatInOutboxManagement ShekayatInbox = new ShekayatInOutboxManagement(db);
            ShekayatInbox.AddShekayatInOutbox(model, profile);
            return result;
        }
        public int ChangeShekayatStatus(Shekayat model, string F_UserID)
        {
            var EditObject = db.Shekayat.FirstOrDefault(u => u.ID == model.ID && u.F_UserID == F_UserID);
            if (EditObject != null)
            {
                EditObject.Status = model.Status;
                db.SaveChanges();
                return 1;
            }
            else
                return 2;
        }

        public int DeleteShekayat(Shekayat model)
        {
            var DeleteObject = db.Shekayat.FirstOrDefault(u => u.ID == model.ID);
            if (DeleteObject != null)
            {
                db.Shekayat.Remove(DeleteObject);
                db.SaveChanges();
                return 1;
            }
            else
                return 2;
        }

        public List<ShekayatModel> ListShekayat(string F_UserID, string SearchString, string ShekayatStatus, int pageNumber, int pageSize, out int total)
        {
            IPagedList<Shekayat> ListObject;
            if (string.IsNullOrEmpty(ShekayatStatus))
            {
                if (string.IsNullOrEmpty(SearchString))
                {
                    ListObject = db.Shekayat.Include(u => u.ShekayatInboxOutBox).Where(u => u.F_UserID == F_UserID).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
                else
                {
                    ListObject = db.Shekayat.Where(m => m.TrackingCode.ToLower().Contains(SearchString.ToLower()) && m.F_UserID == F_UserID).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SearchString))
                {
                    ListObject = db.Shekayat.Where(m => m.Status == ShekayatStatus && m.F_UserID == F_UserID).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
                else
                {

                    ListObject = db.Shekayat.Where(m => m.Status == ShekayatStatus && m.F_UserID == F_UserID).Where(m => m.TrackingCode.ToLower().Contains(SearchString.ToLower())).OrderByDescending(u => u.LastUpdatedOnUTC).ToPagedList(pageNumber, pageSize);
                    total = ListObject.TotalItemCount;
                }
            }
            List<ShekayatModel> list = new List<ShekayatModel>();
            foreach (var ListItem in ListObject)
            {
                ShekayatModel t = new ShekayatModel();
                t.Status = ListItem.Status;
                t.LastUpdateOnUtc = ListItem.LastUpdatedOnUTC ?? default(DateTime);
                t.ID = ListItem.ID;
                t.TrackingCode = ListItem.TrackingCode;
                var TObject = ListItem.ShekayatInboxOutBox.FirstOrDefault(u => u.Type == "Inbox");
                t.Text = TObject != null ? TObject.Text : "";
                var inbox = ListItem.ShekayatInboxOutBox.OrderBy(m => m.CreatedDateOnUtc).FirstOrDefault();
                t.CountInbox = ListItem.ShekayatInboxOutBox.Where(u => u.Type == "Inbox").Count();
                t.CountOutbox = ListItem.ShekayatInboxOutBox.Where(u => u.Type == "OutBox").Count();
                list.Add(t);
            }
            return list;
        }



        public Shekayat ShekayatBrief(int Id, string F_UserId)
        {
            return db.Shekayat.Include(u => u.ShekayatInboxOutBox).FirstOrDefault(m => m.ID == Id && m.F_UserID == F_UserId);
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
                var _object = db.Shekayat.FirstOrDefault(u => u.TrackingCode == temp);
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

        public static List<TicketTrackFactModel> ShekayatTrackFacts(string profile)
        {
            var db = new Entities();
            string F_userId = Tools.UserID(profile);
            ShekayatInOutboxManagement TIM = new ShekayatInOutboxManagement(db);
            var Statuses = (from c in db.Shekayat where c.F_UserID == F_userId group c by c.Status into g select new { FieldCount = g.Count(), FieldName = g.Key }).ToList();
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