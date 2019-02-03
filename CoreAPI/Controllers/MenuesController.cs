using System.Collections.Generic;

using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CoreAPI.Models.DataModel;
using CoreAPI.Models.BLL;
using AutoMapper;
using CoreAPI.Models;
using CoreAPI.CustomFilter;

namespace CoreAPI.Controllers
{

    public class MenuesController : ApiController
    {
        private Entities db = DBConnect.getConnection();

        public MenuesController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Menu, MenuDataModel>();
                cfg.CreateMap<MenuDataModel, Menu>();

                cfg.CreateMap<Menu, ListMenudatamodel>();
                cfg.CreateMap<ListMenudatamodel, Menu>();

                cfg.CreateMap<Menu, MenuParrentDataModel>();
                cfg.CreateMap<MenuParrentDataModel, Menu>();
            });

        }

    


        [Route("api/Menues/GetMenuAndroid")]
        [ResponseType(typeof(List<MenuDataModel>))]
        public async Task<IHttpActionResult> GetMenuAndroid(string username)
        {
            // db.Configuration.LazyLoadingEnabled = false;
            List<MenuDataModel> _list = new List<MenuDataModel>();
            string userId = Tools.UserID(username);
            var menuList = await db.Menu.Where(u => u.isDeleted == false && u.F_UserID == userId && (u.Type == "NoneStaticDynamic" || u.Type == "DynamicPost" || u.Type == "StaticPost")).ToListAsync();
            foreach (var m in menuList)
            {
                _list.Add(Mapper.Map<Menu, MenuDataModel>(m));
            }
            return Ok(_list);
        }


        /// <summary>
        /// لیست منو بر اساس پارامتر های ورودی
        /// </summary>
        /// <param name="username"></param>
        /// <param name="lang"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("api/Menues/GetMenuAll")]
        [ResponseType(typeof(List<MenuDataModel>))]
        public IHttpActionResult GetMenuAll(string username, [FromUri] List<string> lang, bool? status, [FromUri] List<string> type)
        {
            List<MenuDataModel> _list = new List<MenuDataModel>();
            string userId = Tools.UserID(username);
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
                var menuList = db.Menu.Where(u => u.isDeleted == false && (!lang.Any() | lang.Contains(u.Language)) && (status == null | u.Status == status) && (!type.Any() | type.Contains(u.Type)) && u.F_UserID == userId).OrderBy(u => u.Weight).ToList();
                foreach (var m in menuList)
                {
                    _list.Add(Mapper.Map<Menu, MenuDataModel>(m));
                }
        
            return Ok(_list);
        }

        /// <summary>
        /// لیست والد های یک منو
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Menues/GetParentMenues")]
        [ResponseType(typeof(List<MenuParrentDataModel>))]
        public async Task<IHttpActionResult> GetParentMenues(int id)
        {
            List<MenuParrentDataModel> _list = new List<MenuParrentDataModel>();
            Entities lazydb = DBConnect.getEnabledLazyConnection();

            var menu = await lazydb.Menu.Include(u => u.Menu2).FirstOrDefaultAsync(u => u.isDeleted == false && u.Status == true && u.ID == id);
            if (menu == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var parrentlist = Mapper.Map<Menu, MenuParrentDataModel>(menu);
            return Ok(parrentlist);
        }
        /// <summary>
        /// جزییات یک منو
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Menues/GetMenu")]
        //[Authorize(Roles = "CMSBase")]
        [Authorize]
        [ResponseType(typeof(MenuDataModel))]
        public async Task<IHttpActionResult> GetMenu(int id)
        {
            string userId = Tools.RootUserID();
            var menu = await db.Menu.Where(u => u.F_UserID == userId && u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();
            if (menu == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok(Mapper.Map<Menu, MenuDataModel>(menu));
        }


        /// <summary>
        /// جزییات یک منو بر اساس نوع
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        [Route("api/Menues/GetFormByType")]
        [ResponseType(typeof(MenuDataModel))]
        public async Task<IHttpActionResult> GetFormByType(string Type,string profile)
        {
            string userId = Tools.UserID(profile);
            var menu = await db.Menu.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Description == Type).FirstOrDefaultAsync();
            if (menu == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok(Mapper.Map<Menu, MenuDataModel>(menu));
        }

        /// <summary>
        /// تعداد پست های یک منو
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns></returns>
        [Route("api/Menues/GetMenuPostCount")]
        [ResponseType(typeof(MenuDataModel))]
        public async Task<IHttpActionResult> GetMenuPostCount(int menuid)
        {
            int? f_menuid = (from b in db.Menu where b.ID == menuid select b.F_MenuID).FirstOrDefault();
            if (f_menuid == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var menu = await (from a in db.Menu where a.F_MenuID == f_menuid & a.isDeleted == false & a.Status == true select new { parrentmenuname = a.Menu2.Name, menupostcount = a.Posts.Count(), name = a.Name, id = a.ID }).ToListAsync();
            ListMenuPostCountDataModel menupost = new ListMenuPostCountDataModel();
            MenuPostCountDataModel counter;
            menupost.Name = menu.FirstOrDefault().parrentmenuname;
            foreach (var m in menu)
            {
                counter = new MenuPostCountDataModel();
                counter.MenuName = m.name;
                counter.PostCount = m.menupostcount;
                counter.MenuId = m.id;
                menupost.MenuCount.Add(counter);
            }

            return Ok(menupost);
        }






        private bool MenuExists(int id)
        {
            string userId = Tools.UserID();
            return db.Menu.Count(e => e.ID == id && e.F_UserID == userId && e.isDeleted == false) > 0;
        }



        /// <summary>
        /// حذف منو
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Menues/DeleteMenu")]
        [Authorize(Roles = "CMSBase")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteMenu(int id)
        {
            if (!MenuExists(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            string userid = Tools.UserID();
            var menu = await db.Menu.Where(u => u.ID == id && u.F_UserID == userid).FirstOrDefaultAsync();
            menu.isDeleted = true;
            CascadeTools ct = new CascadeTools();
            foreach (var p in menu.Posts.ToList())
            {
                p.isDeleted = true;
                foreach (var c in p.Comments.ToList())
                {
                    ct.DeleteCascadeComments(c, db);
                }
                db.Entry(p).State = EntityState.Modified;

            }
            foreach (var sub in menu.Menu1.ToList())
            {
                sub.F_MenuID = null;
                db.Entry(sub).State = EntityState.Modified;

            }
            db.Entry(menu).State = EntityState.Modified;

           await db.SaveChangesAsync();
            return Ok();

        }




        /// <summary>
        /// ویرایش منو وب سایت
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menu"></param>
        /// <returns></returns>
        [Route("api/Menues/PutMenu")]
        [Authorize(Roles = "CMSBase")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMenu(int id, MenuDataModel menu)
        {
            if (id != menu.ID)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            if (!MenuExists(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var _menu = await db.Menu.FindAsync(id);
            Mapper.Map(menu, _menu);

            db.Entry(_menu).State = EntityState.Modified;
            db.Entry(_menu).Property(x => x.F_UserID).IsModified = false;
            db.Entry(_menu).Property(x => x.isDeleted).IsModified = false;
            db.Entry(_menu).Property(x => x.Type).IsModified = false;
            await db.SaveChangesAsync();
            return Ok();


        }



        /// <summary>
        ///  تغییر وضعیت نمایش منو
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Menues/Putchangestatus")]
        [Authorize(Roles = "CMSBase")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putchangestatus(int id)
        {
            if (!MenuExists(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var menu = await db.Menu.FindAsync(id);
            menu.Status = !menu.Status;
            CascadeTools ct = new CascadeTools();
            foreach (var m in menu.Menu1.ToList())
            {
                ct.CascadeChangeStatusMenu(m, menu.Status, db);
            }
            db.Entry(menu).State = EntityState.Modified;
          await  db.SaveChangesAsync();
            return Ok();

        }


        /// <summary>
        /// افزودن منوی جدید به وب سایت
        /// </summary>
        /// <param name="menu"></param>
        /// <returns> Id 
        /// رکورد جدید </returns>
        [Route("api/Menues/PostMenu")]
        [Authorize(Roles = "CMSBase")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> PostMenu(MenuDataModel menu)
        {
            Menu _menu = Mapper.Map<MenuDataModel, Menu>(menu);
            _menu.F_UserID = Tools.UserID();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Menu.Add(_menu);
            await db.SaveChangesAsync();
            return Ok(_menu.ID);
        }




        /// <summary>
        /// حذف منو به صورت آبشاری
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Menues/DeleteMenucascade")]
        [Authorize(Roles = "CMSBase")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteMenucascade(int id)
        {
            if (!MenuExists(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            string userid = Tools.UserID();
            var menu = await db.Menu.Where(u => u.ID == id && u.F_UserID == userid).FirstOrDefaultAsync();
            menu.isDeleted = true;
            CascadeTools ct = new CascadeTools();
            foreach (var m in menu.Menu1.ToList())
            {
                ct.CascadeDeleteMenu(m, db);
            }
            foreach (var m in menu.Menu1.ToList())
            {
                ct.CascadeDeleteFanbazarMenu(m, db);
            

            }
            db.Entry(menu).State = EntityState.Modified;
           await db.SaveChangesAsync();
            return Ok();

        }



    }
}