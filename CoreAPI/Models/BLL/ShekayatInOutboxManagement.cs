using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoreAPI.Models.BLL
{
    public class ShekayatInOutboxManagement
    {
        private Entities db { get; set; }
        public ShekayatInOutboxManagement(Entities _db)
        {
            db = _db;
        }
        public int AddShekayatInOutbox(ShekayatModel model, string profile)
        {
            ShekayatInboxOutBox InsertObject = new ShekayatInboxOutBox();
            InsertObject.CreatedDateOnUtc = DateTime.Now;
            InsertObject.F_ShekayatID = model.F_ShekayatID;
            InsertObject.Text = model.Text;
            InsertObject.Type = model.Type;
            db.ShekayatInboxOutBox.Add(InsertObject);
            db.SaveChanges();
            return 1;
        }

        public List<ShekayatInboxOutBox> ListShekayatInbox(string F_UserID, int F_Shekayat_Id)
        {
            var ListObject = db.ShekayatInboxOutBox.Include(m => m.Shekayat).Where(m => m.F_ShekayatID == F_Shekayat_Id && m.Shekayat.F_UserID == F_UserID);
            List<ShekayatInboxOutBox> list = new List<ShekayatInboxOutBox>();
            foreach (var ListItem in ListObject)
            {
                ShekayatInboxOutBox t = new ShekayatInboxOutBox();
                t.CreatedDateOnUtc = ListItem.CreatedDateOnUtc ?? default(DateTime);
                t.F_ShekayatID = ListItem.F_ShekayatID ?? default(int);
                t.Text = ListItem.Text;
                t.Type = ListItem.Type;
                t.ID = ListItem.ID;
                list.Add(t);
            }
            return list.OrderByDescending(u=>u.CreatedDateOnUtc).ToList();
        }
    }
}