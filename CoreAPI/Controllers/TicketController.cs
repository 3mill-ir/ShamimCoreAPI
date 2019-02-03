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
    public class TicketController : ApiController
    {
        Entities db = DBConnect.getConnection();
        #region Client
        [HttpPost]
        [Route("api/Ticket/PostTicket")]
        public IHttpActionResult PostTicket(UserTicketModel model)
        {
            var db = new Entities();
            TicketManagement Tickets = new TicketManagement(model.profile, db);
            if (!string.IsNullOrEmpty(model.TrackingCode))
            {
                string F_UserId = Tools.UserID(model.profile);
                var IsTicketExists = db.Ticket.FirstOrDefault(u => u.TrackingCode == model.TrackingCode && u.F_UserID == F_UserId);
                if (IsTicketExists != null)
                {
                    IsTicketExists.LastUpdatedOnUTC = DateTime.Now;
                    model.ID = IsTicketExists.ID;
                    db.SaveChanges();
                }
            }
            else
            {
                model.ID = 0;
            }
            string Scale = Tickets.AddTicket(model.profile, model);
            return Ok(Scale);
        }
        [Route("api/Ticket/GetTicketTracking")]
        public IHttpActionResult GetTicketTracking(string profile, string TicketId)
        {
            var db = new Entities();
            string F_UserId = Tools.UserID(profile);
            var ListObject = db.Ticket.FirstOrDefault(m => m.TrackingCode == TicketId && m.F_UserID == F_UserId);
            if (ListObject == null)
                return NotFound();
            TicketInboxManagement TIM = new TicketInboxManagement(db);
            List<TicketInboxModel> model = TIM.ListTicketInbox(F_UserId, ListObject.ID).ToList();
            if (model.Count < 1)
                return NotFound();
            return Ok(model);
        }
        #endregion



        #region Admin
        [Route("api/Ticket/GetTicketAccessory")]
        public IHttpActionResult GetTicketAccessory()
        {
            string F_UserId = Tools.UserID();
            int UnResponseTicket = db.Ticket.Where(u => u.Status == "در وضعیت انتظار" && u.F_UserID == F_UserId).Count();
            int ToResponseTicket = db.Ticket.Where(u => u.Status == "در حال بررسی" && u.F_UserID == F_UserId).Count();
            int ResponseTicket = db.Ticket.Where(u => u.Status == "پاسخ داده شده" && u.F_UserID == F_UserId).Count();
            return Ok(new TicketAccessoryDataModel() { UnResponseTicketCount = UnResponseTicket, ResponseTicketCount = ResponseTicket, ToResponseTicketCount = ToResponseTicket, AllTicketCount = ResponseTicket + ToResponseTicket + UnResponseTicket });
        }
        [Route("api/Ticket/GetTicketList")]
        public IHttpActionResult GetTicketList(int page, string currentFilter, string searchString, string TicketStatus)
        {
            string F_UserId = Tools.RootUserID();
            string profile = Tools.RootUserName(F_UserId);
            TicketManagement Ticket = new TicketManagement(profile, db);
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            int total;
            var pagedList = Ticket.ListTicket(F_UserId, searchString, TicketStatus, page, 10, out total).OrderByDescending(u => u.LastUpdateOnUtc);
            return Ok(new TicketListDataModel() { Tickets = pagedList.ToList(), Total = total });
        }
        [Route("api/Ticket/GetTicketDetail")]
        public IHttpActionResult GetTicketDetail(int F_TicketId)
        {
            TicketInboxManagement Ticket = new TicketInboxManagement(db);
            string F_UserId = Tools.RootUserID();
            var Result = Ticket.ListTicketInbox(F_UserId, F_TicketId).OrderByDescending(u => u.CreatedOnUTC);
            return Ok(Result);
        }
        [HttpPost]
        [Route("api/Ticket/PostTicketResponse")]
        public IHttpActionResult PostTicketResponse(TicketOutBoxModel model, int F_LastTicketInboxId)
        {
            if (string.IsNullOrEmpty(model.Content_One))
                ModelState.AddModelError("Content_One", "لطفاً فیلد های خالی را پر نمایید");
            if (ModelState.IsValid)
            {
                TicketOutBoxManagement Outbox = new TicketOutBoxManagement(db);
                string F_UserId = Tools.RootUserID();
                Outbox.AddTicketOutBox(model, F_UserId);
                return Ok();
            }
            return InternalServerError();
        }
        [Route("api/Ticket/GetTicketStatus")]
        public IHttpActionResult GetTicketStatus(int F_TicketId)
        {
            string profile = Tools.RootUserName(Tools.RootUserID());
            TicketManagement ticket = new TicketManagement(profile, db);
            TicketModel model = new TicketModel();
            model.ID = F_TicketId;
            model.Status = ticket.TicketBrief(F_TicketId, profile).Status;
            return Ok(model);
        }
        [HttpPost]
        [Route("api/Ticket/PostTicketChangeStatus")]
        public IHttpActionResult PostTicketChangeStatus(TicketModel model, int F_TicketId)
        {
            string F_UserId = Tools.RootUserID();
            string profile = Tools.RootUserName(F_UserId);
            TicketManagement ticket = new TicketManagement(profile, db);
            ticket.ChangeTicketStatus(model, F_UserId);
            return Ok();
        }

        #endregion
    }
}