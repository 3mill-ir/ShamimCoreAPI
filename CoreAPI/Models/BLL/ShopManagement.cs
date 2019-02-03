using CoreAPI.Models.DataModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Http;

namespace CoreAPI.Models.BLL
{
    public class ShopManagement
    {
        private Entities db { get; set; }
        private Entities ldb { get; set; }

        public ShopManagement(Entities _db)
        {
            db = _db;
            ldb = DBConnect.getEnabledLazyConnection();
        }
        public string AddShop(string profile, List<ShopItem> model)
        {
            try
            {
                string F_UserID = Tools.UserID(); int F_ShopID = 0;
                var Found = db.Shop.Include(u => u.ShopItem).FirstOrDefault(u => u.State == "Initiated" && u.F_UserID == F_UserID);
                if (Found == null)
                {
                    Shop ShopModel = new Shop();
                    ShopModel.CreatedDateOnUtc = DateTime.Now;
                    ShopModel.F_UserID = F_UserID;
                    ShopModel.ParentUserID = Tools.UserID(profile);
                    ShopModel.State = "Initiated";
                    db.Shop.Add(ShopModel);
                    db.SaveChanges();
                    F_ShopID = ShopModel.ID;
                }
                else
                    F_ShopID = Found.ID;
                foreach (var item in model)
                {
                    var Item = db.Item.FirstOrDefault(u => u.ID == item.F_ItemID);
                    if (Item.Stock > 0)
                    {
                        Item.Stock = Item.Stock - 1;
                        var temp = Found.ShopItem.FirstOrDefault(u => u.F_ItemID == item.F_ItemID);
                        if (temp != null)
                            temp.Count = temp.Count + item.Count;
                        else
                        {
                            item.CreatedDateOnUtc = DateTime.Now;
                            item.F_ShopID = F_ShopID;
                            db.ShopItem.Add(item);
                        }
                        Found.TotalPrice = (double.Parse(string.IsNullOrEmpty(Found.TotalPrice) ? "0" : Found.TotalPrice) + (double.Parse(Item.Price))).ToString();
                        db.SaveChanges();
                    }
                }
                return "OK";
            }
            catch { return "NOK"; }
        }
        public string EditShop(Shop model)
        {
            string F_UserID = Tools.UserID();
            var EditObject = db.Shop.FirstOrDefault(u => u.ID == model.ID && u.F_UserID == F_UserID);
            if (EditObject != null)
            {
                if (!string.IsNullOrEmpty(model.DeliveryType))
                    EditObject.DeliveryType = model.DeliveryType;
                if (!string.IsNullOrEmpty(model.DeliveryCost))
                    EditObject.DeliveryCost = model.DeliveryCost;
                if (!string.IsNullOrEmpty(model.PayType))
                    EditObject.PayType = model.PayType;
                if (model.Payed != null)
                    EditObject.Payed = model.Payed;
                if (!string.IsNullOrEmpty(model.State))
                {
                    if (model.State == "Delivered")
                        EditObject.DeliveryTime = DateTime.Now;
                    if (model.State == "Ordered")
                        EditObject.OrderedDate = DateTime.Now;
                    EditObject.State = model.State;
                }
                db.SaveChanges();
                return "OK";
            }
            else
                return "NOK";
        }
        public string EditShopItem(List<ShopItem> model)
        {
            string F_UserID = Tools.UserID();
            int? fp = 0;
            var EditObjects = db.Shop.Include("ShopItem.Item").FirstOrDefault(u => u.F_UserID == F_UserID);
            if (EditObjects != null)
            {
                foreach (var item in model)
                {
                    var temp = EditObjects.ShopItem.FirstOrDefault(u => u.ID == item.ID);
                    if (temp.Item.Stock >= item.Count)
                    {
                        fp = item.Count - temp.Count;
                        temp.Count = item.Count;
                        EditObjects.TotalPrice = (int.Parse(EditObjects.TotalPrice) + (fp * int.Parse(temp.Item.Price))).ToString();
                        temp.Item.Stock -= fp;
                        db.SaveChanges();
                    }
                }
                return "OK";
            }
            else
                return "NOK";
        }
        public string ChangeShopState(Shop model)
        {
            string F_UserID = Tools.UserID();
            var EditObject = db.Shop.FirstOrDefault(u => u.ID == model.ID && (u.F_UserID == F_UserID||u.ParentUserID==F_UserID));
            if (EditObject != null)
            {
                if (model.State == "Delivered")
                    EditObject.DeliveryTime = DateTime.Now;
                if (model.State == "Ordered")
                    EditObject.OrderedDate = DateTime.Now;
                EditObject.State = model.State;
                db.SaveChanges();
                return "OK";
            }
            else
                return "NOK";
        }

        public int DeleteShop(int F_ShopID)
        {
            var DeleteObject = db.ShopItem.Include(q => q.Item).FirstOrDefault(u => u.ID == F_ShopID);
            if (DeleteObject != null)
            {
                var Item = db.Item.FirstOrDefault(u => u.ID == DeleteObject.F_ItemID);
                Item.Stock = Item.Stock + DeleteObject.Count;
                var temp = db.Shop.FirstOrDefault(u => u.ID == DeleteObject.F_ShopID);
                if (temp != null && temp.ShopItem.Count == 1)
                    db.Shop.Remove(DeleteObject.Shop);
                else
                    temp.TotalPrice = (double.Parse(temp.TotalPrice) - (double.Parse(Item.Price) * DeleteObject.Count)).ToString();
                db.ShopItem.Remove(DeleteObject);
                db.SaveChanges();
                return 1;
            }
            else
                return 2;
        }
        public ShopPagedList ListShop(string F_UserID, string ShopStatus, int pageNumber, int pageSize,string profile)
        {
            string ParrentUserID = Tools.UserID(profile);
            var temp = db.Shop.AsNoTracking().Include("ShopItem.Item").Where(u => u.F_UserID == F_UserID && u.State == ShopStatus&&u.ParentUserID== ParrentUserID).OrderBy(u => u.CreatedDateOnUtc).ToPagedList(pageNumber, pageSize);
            return new ShopPagedList() { ShopList = temp.ToList(), Total = temp.TotalItemCount };
        }

        public ShopPagedList ListShopForAdmin(string ShopStatus, int pageNumber, int pageSize)
        {
            string ParrentUserID = Tools.UserID();
            var temp = db.Shop.AsNoTracking().Include("ShopItem.Item").Where(q => (string.IsNullOrEmpty(ShopStatus) | q.State == ShopStatus)&&q.ParentUserID==ParrentUserID).OrderBy(u => u.CreatedDateOnUtc).ToPagedList(pageNumber, pageSize);
            return new ShopPagedList() { ShopList = temp.ToList(), Total = temp.TotalItemCount };
        }
        public Shop ShopDetail(int ShopID)
        {
            var temp = db.Shop.AsNoTracking().Include("ShopItem.Item").FirstOrDefault(u => u.ID == ShopID);
            return temp;
        }
    }
}