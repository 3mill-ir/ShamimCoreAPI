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
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CoreAPI.Controllers
{
    //controller of languages

    public class UtilityController : ApiController
    {
        private Entities db = DBConnect.getConnection();
        // private Entities db = new Entities();
        public UtilityController()
        {
            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<LanguageDataModel, Languages>();
                cfg.CreateMap<Languages, LanguageDataModel>();
                cfg.CreateMap<UserInformation, UserInformationDataModel>();
                cfg.CreateMap<UserInformationDataModel, UserInformation>();

                cfg.CreateMap<AddressState, AddressDataModel>();
                cfg.CreateMap<AddressDataModel, AddressState>();

                cfg.CreateMap<AddressCity, AddressDataModel>();
                cfg.CreateMap<AddressDataModel, AddressCity>();

            });
        }
        /// <summary>
        /// لیست تمامی زبانها
        /// </summary>
        /// <returns></returns>
        [Route("api/Utility/GetLanguages")]
        [ResponseType(typeof(LanguageDataModel))]
        public async Task<IHttpActionResult> GetLanguages()
        {
            List<LanguageDataModel> _Langlist = new List<LanguageDataModel>();
            var LangList = await db.Languages.OrderBy(u => u.Weight).ToListAsync();
            foreach (var m in LangList)
            {
                _Langlist.Add(Mapper.Map<Languages, LanguageDataModel>(m));
            }
            return Ok(_Langlist);
        }

        //Http:GET:
        [Route("api/Utility/GetLanguage")]
        [ResponseType(typeof(LanguageDataModel))]
        public async Task<IHttpActionResult> GetLanguage(string lang)
        {
            string userId = Tools.UserID();
            var Lang = await db.Languages.Where(u => u.Language == lang).FirstOrDefaultAsync();
            if (Lang == null)
            {
                return Content(HttpStatusCode.NotFound, "زبان مود نظر شما پیدا نشد");
            }
            return Ok(Mapper.Map<Languages, LanguageDataModel>(Lang));
        }
        //Post
        [ResponseType(typeof(LanguageDataModel))]
        public async Task<IHttpActionResult> PostLanguage(LanguageDataModel lang)
        {
            Languages _Lang = Mapper.Map<LanguageDataModel, Languages>(lang);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Languages.Add(_Lang);
            await db.SaveChangesAsync();
            return Ok("... رکورد شما با موفقیت ثبت شد");


        }
        [ResponseType(typeof(FanbazarFactDataModel))]
        public IHttpActionResult getFacts()
        {
            FanbazarFactDataModel facts = new FanbazarFactDataModel();
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var context = new ApplicationDbContext();
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleMngr = new RoleManager<IdentityRole>(roleStore);

            var userRole = roleMngr.Roles.Where(u => u.Name == "FanbazarUser").FirstOrDefault();
            var users = userRole.Users.Count;
            var companies = db.Item.Count(u => u.Type == "Company");
            var offeritem = db.Item.Count(u => u.Type == "Offer");
            var demanditem = db.Item.Count(u => u.Type == "Demand");
            facts.CompanyCount = companies;
            facts.OfferCount = offeritem;
            facts.DemandCount = demanditem;
            facts.UserCount = users;
            return Ok(facts);
        }
        //method has been removed and other members of group have been informed, it was utilized for Fanbazar
        /*  [AllowAnonymous]
          public async Task<IHttpActionResult> getLatestCompanies(int count)
          {
              List<IdentityUser> _users = new List<IdentityUser>();
              ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
              var context = new ApplicationDbContext();
              var roleStore = new RoleStore<IdentityRole>(context);
              var roleMngr = new RoleManager<IdentityRole>(roleStore);
              var companyrole = roleMngr.Roles.Where(u => u.Name == "Company").FirstOrDefault();
              var users = companyrole.Users.OrderByDescending(u=>u.UserId);
              foreach (var item in users)
              {
                  var user = await usermanager.FindByIdAsync(item.UserId);
                  _users.Add(user);
              }
              return Ok(_users.Take(count));
          }*/
        [Route("api/Utility/GetFanbazarUsers")]
        public async Task<IHttpActionResult> GetFanbazarUsers()
        {
            string User_ID = Tools.RootUserID();
            List<ProfileRegisterDataModel> _users = new List<ProfileRegisterDataModel>();
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var context = new ApplicationDbContext();
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleMngr = new RoleManager<IdentityRole>(roleStore);
            var fanbazarrole = roleMngr.Roles.Where(u => u.Name == "FanbazarUser").FirstOrDefault();
            var users = fanbazarrole.Users;
            var profiles = db.UserInformation.Where(u => u.F_ParrentUserID==User_ID);
            foreach (var item in users)
            {
                ProfileRegisterDataModel prof = new ProfileRegisterDataModel();
                var user = await usermanager.FindByIdAsync(item.UserId);
                var profile = profiles.FirstOrDefault(u => u.F_UserID == item.UserId);
                if (profile != null)
                {
                    UserInformationDataModel _profile = Mapper.Map<UserInformation, UserInformationDataModel>(profile);
                    prof.Email = user.Email;
                    prof.UserName = user.UserName;
                    prof.RoleName = "FanbazarUser";
                    prof.Profile = _profile;
                    _users.Add(prof);
                }
            }

            return Ok(_users);
        }


        /*  protected override void Dispose(bool disposing)
          {
              if (disposing)
              {
                  db.Dispose();
              }
              base.Dispose(disposing);
          }*/

        /// <summary>
        /// لیست تمامی استانها
        /// </summary>
        /// <returns></returns>
        [Route("api/Utility/GetStates")]
        [ResponseType(typeof(LanguageDataModel))]
        public async Task<IHttpActionResult> GetStates()
        {
            List<AddressDataModel> _Addlist = new List<AddressDataModel>();
            var AddList = await db.AddressState.Where(u => u.isDeleted == false && u.Status == true).ToListAsync();
            foreach (var m in AddList)
            {
                _Addlist.Add(Mapper.Map<AddressState, AddressDataModel>(m));
            }
            return Ok(_Addlist);
        }

        /// <summary>
        /// لیست تمامی استانها
        /// </summary>
        /// <returns></returns>
        [Route("api/Utility/GetCity")]
        [ResponseType(typeof(AddressDataModel))]
        public async Task<IHttpActionResult> GetCity(int stateId)
        {
            List<AddressDataModel> _Addlist = new List<AddressDataModel>();
            var AddList = await db.AddressCity.Where(u => u.isDeleted == false && u.Status == true && u.F_StateID == stateId).ToListAsync();
            foreach (var m in AddList)
            {
                _Addlist.Add(Mapper.Map<AddressCity, AddressDataModel>(m));
            }
            return Ok(_Addlist);
        }
    }
}
