using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CoreAPI.Models.DataModel;
using Microsoft.AspNet.Identity;
using CoreAPI.Models.BLL;
using AutoMapper;
using CoreAPI.Models;

namespace CoreAPI.Controllers
{
    /// <summary>
    /// تنظیم قالب پنل مدیریت
    /// </summary>
    [Authorize]
    public class AdminPanelConfigsController : ApiController
    {
         private Entities db = DBConnect.getConnection();

        public AdminPanelConfigsController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<PageConfigsModel, AdminPanelConfig>();
                cfg.CreateMap<AdminPanelConfig, PageConfigsModel>();
            });
        }

        /// <summary>
        /// فراخوانی تنظیمات قالب مدیریت
        /// </summary>
        /// <returns>بازگشت دیتا مدل
        /// PageConfigsModel</returns>
        [ResponseType(typeof(PageConfigsModel))]
        public async Task<IHttpActionResult> GetAdminPanelConfig()
        {
            string userId = Tools.UserID();
            AdminPanelConfig adminPanelConfig = await db.AdminPanelConfig.FirstOrDefaultAsync(u=>u.F_UserID== userId);
            if (adminPanelConfig == null)
            {
                return NotFound();
            }
            var pageConfigsModel = Mapper.Map<AdminPanelConfig, PageConfigsModel>(adminPanelConfig);
            return Ok(pageConfigsModel);
        }



        /// <summary>
        /// اعمال تغییر در قالب پنل مدیریت
        /// </summary>
        /// <param name="pageConfigsModel"> ارسال مدل 
        /// pageConfigsModel</param>
        /// <returns>بازگشت 
        /// ok
        /// در صورت انجام صحیح عملبات ذخیره سازی و بازگشت
        /// Nok 
        /// در صورت مشکل در عملیات ذخیره سازی</returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostAdminPanelConfig(PageConfigsModel pageConfigsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var adminPanelConfig = Mapper.Map<PageConfigsModel, AdminPanelConfig>(pageConfigsModel);
            string userId = Tools.UserID();
            if (AdminPanelConfigExists(userId))
            {              
                var admin = db.AdminPanelConfig.Find(userId);
                admin.F_UserID = userId;
                Mapper.Map(pageConfigsModel, admin);
                
                db.Entry(admin).State = EntityState.Modified;
            }
            else
            {
                adminPanelConfig.F_UserID = Tools.UserID();
                db.AdminPanelConfig.Add(adminPanelConfig);
            }

            await db.SaveChangesAsync();
            return   Ok();
        }


        private bool AdminPanelConfigExists(string UserID)
        {
            return db.AdminPanelConfig.Count(e => e.F_UserID == UserID) > 0;
        }
    }
}