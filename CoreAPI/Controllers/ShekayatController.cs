using AutoMapper;
using CoreAPI.Models;
using CoreAPI.Models.BLL;
using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq.Dynamic;
using System.Collections.ObjectModel;
namespace CoreAPI.Controllers
{
    public class ShekayatController : ApiController
    {
        Entities db = DBConnect.getConnection();
        #region Client
        [HttpPost]
        [Route("api/Shekayat/PostShekayat")]
        public IHttpActionResult PostShekayat(ShekayatModel model)
        {
            model.Type = "Inbox";
            var db = new Entities();
            ShekayatManagement Shekayats = new ShekayatManagement(db);
            if (!string.IsNullOrEmpty(model.TrackingCode))
            {
                string F_UserId = Tools.UserID(model.profile);
                var IsShekayatExists = db.Shekayat.FirstOrDefault(u => u.TrackingCode == model.TrackingCode && u.F_UserID == F_UserId);
                if (IsShekayatExists != null)
                {
                    IsShekayatExists.LastUpdatedOnUTC = DateTime.Now;
                    model.ID = IsShekayatExists.ID;
                    db.SaveChanges();
                }
            }
            else
            {
                model.ID = 0;
            }
            string Scale = Shekayats.AddShekayat(model.profile, model);
            return Ok(Scale);
        }
        [Route("api/Shekayat/GetShekayatTracking")]
        public IHttpActionResult GetShekayatTracking(string profile, string ShekayatId)
        {
            var db = new Entities();
            string F_UserId = Tools.UserID(profile);
            var ListObject = db.Shekayat.FirstOrDefault(m => m.TrackingCode == ShekayatId && m.F_UserID == F_UserId);
            if (ListObject == null)
                return NotFound();
            ShekayatInOutboxManagement TIM = new ShekayatInOutboxManagement(db);
            List<ShekayatInboxOutBox> model = TIM.ListShekayatInbox(F_UserId, ListObject.ID).ToList();
            if (model.Count < 1)
                return NotFound();
            return Ok(model);
        }
        #endregion



        #region Admin
        [Route("api/Shekayat/GetShekayatAccessory")]
        public IHttpActionResult GetShekayatAccessory()
        {
            string F_UserId = Tools.RootUserID();
            int UnResponseShekayat = db.Shekayat.Where(u => u.Status == "در وضعیت انتظار" && u.F_UserID == F_UserId).Count();
            int ToResponseShekayat = db.Shekayat.Where(u => u.Status == "در حال بررسی" && u.F_UserID == F_UserId).Count();
            int ResponseShekayat = db.Shekayat.Where(u => u.Status == "پاسخ داده شده" && u.F_UserID == F_UserId).Count();
            return Ok(new TicketAccessoryDataModel() { UnResponseTicketCount = UnResponseShekayat, ResponseTicketCount = ResponseShekayat, ToResponseTicketCount = ToResponseShekayat, AllTicketCount = ResponseShekayat + ToResponseShekayat + UnResponseShekayat });
        }
        [Route("api/Shekayat/GetShekayatList")]
        public IHttpActionResult GetShekayatList(int page, string currentFilter, string searchString, string ShekayatStatus)
        {
            string F_UserId = Tools.RootUserID();
            ShekayatManagement Shekayat = new ShekayatManagement(db);
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            int total;
            var pagedList = Shekayat.ListShekayat(F_UserId, searchString, ShekayatStatus, page, 10, out total).OrderByDescending(u => u.LastUpdateOnUtc);
            return Ok(new ShekayatListDataModel() { Shekayats = pagedList.ToList(), Total = total });
        }
        [Route("api/Shekayat/GetShekayatDetail")]
        public IHttpActionResult GetShekayatDetail(int F_ShekayatId)
        {
            string F_UserId = Tools.RootUserID();
            var Result = db.Shekayat.Include(u => u.ShekayatInboxOutBox).FirstOrDefault(m => m.ID == F_ShekayatId && m.F_UserID == F_UserId);
            return Ok(Result);
        }
        [HttpPost]
        [Route("api/Shekayat/PostShekayatResponse")]
        public IHttpActionResult PostShekayatResponse(ShekayatModel model, int F_LastShekayatInboxId)
        {
            model.Type = "OutBox";
            if (string.IsNullOrEmpty(model.Text))
                ModelState.AddModelError("Content_One", "لطفاً فیلد های خالی را پر نمایید");
            if (ModelState.IsValid)
            {
                ShekayatInOutboxManagement Outbox = new ShekayatInOutboxManagement(db);
                string F_UserId = Tools.RootUserID();
                Outbox.AddShekayatInOutbox(model, F_UserId);
                return Ok();
            }
            return InternalServerError();
        }
        [Route("api/Shekayat/GetShekayatStatus")]
        public IHttpActionResult GetShekayatStatus(int F_ShekayatId)
        {
            string profile = Tools.RootUserName(Tools.RootUserID());
            ShekayatManagement Shekayat = new ShekayatManagement(db);
            ShekayatModel model = new ShekayatModel();
            model.ID = F_ShekayatId;
            model.Status = Shekayat.ShekayatBrief(F_ShekayatId, profile).Status;
            return Ok(model);
        }
        [HttpPost]
        [Route("api/Shekayat/PostShekayatChangeStatus")]
        public IHttpActionResult PostShekayatChangeStatus(Shekayat model, int F_ShekayatId)
        {
            string F_UserId = Tools.RootUserID();
            string profile = Tools.RootUserName(F_UserId);
            ShekayatManagement Shekayat = new ShekayatManagement(db);
            Shekayat.ChangeShekayatStatus(model, F_UserId);
            return Ok();
        }
        #endregion
    }
}