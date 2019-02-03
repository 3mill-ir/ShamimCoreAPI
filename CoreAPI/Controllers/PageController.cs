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

namespace CoreAPI.Controllers
{
   
    public class PageController: ApiController
    {
        private Entities db = DBConnect.getConnection();
        // private Entities db = new Entities();
        public PageController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Menu, PageDataModel>();
                cfg.CreateMap<PageDataModel, Menu>();
            });
        }
        /*[Route("api/Page/PostPage")]
        [ResponseType(typeof(PageDataModel))]
        public async Task<IHttpActionResult> PostPage(PageDataModel p)
        {
            Pages _page = Mapper.Map<PageDataModel, Pages>(p);
            _page.isDeleted = false;
            _page.F_UserID = Tools.UserID();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Pages.Add(_page);
            await db.SaveChangesAsync();
            return Ok("با موفقیت ثبت شد");

        }
        [Route("api/Page/PutPage")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult>PutPage(int id,PageDataModel p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != p.ID)
            {
                return Content(HttpStatusCode.BadRequest, "اطلاعات ورودی همخوانی ندارند");
            }
            if (!PageExists(id))
            {
                return Content(HttpStatusCode.NotFound, "صفحه با آیدی مورد نظر پیدا نشد");
            }

            var _page = db.Pages.FindAsync(id);// Mapper.Map<PageDataModel, Pages>(p);
            Pages page = await _page;

             Mapper.Map(p, page);
            // _page.F_UserID = Tools.UserID();
            db.Entry(page).State = EntityState.Modified;
            db.Entry(page).Property(x => x.F_UserID).IsModified = false;
            await db.SaveChangesAsync();
            return Ok("صفحه مورد نظر با موفقیت ویرایش شد");
        }*/


            /// <summary>
            /// ویرایش و یا افزودن پیج استاتیک 
            /// </summary>
            /// <param name="menuid"></param>
            /// <param name="p"></param>
            /// <returns></returns>
        [Route("api/Page/PutPostPage")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPostPage(int menuid, PageDataModel p)
        {
            string userId = Tools.RootUserID();
            var menu = await db.Menu.Where(u => u.F_UserID == userId && u.isDeleted == false && u.ID == menuid).FirstOrDefaultAsync();
            if (menu == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            menu.ActionContent = p.ActionContent;
            db.Entry(menu).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok();
            //   var _page = db.Pages.Where(u => u.F_UserID == userId && u.F_MenuID == menuid ).FirstOrDefault();
            // Mapper.Map<PageDataModel, Pages>(p);
            //var _page = menu.Pages.FirstOrDefault();
            //if (_page != null)
            //{
            //    if (string.IsNullOrEmpty(_page.PageType) | _page.PageType == "UserPage")
            //    {
            //        Mapper.Map(p, _page);
            //        _page.F_MenuID = menuid;
            //        _page.F_UserID = userId;
            //        _page.isDeleted = false;
            //        /* page.ID = _page.ID;
            //         page.F_UserID = _page.F_UserID;*/
            //        // _page.F_UserID = Tools.UserID();
            //        db.Entry(_page).State = EntityState.Modified;
            //        db.Entry(_page).Property(x => x.F_UserID).IsModified = false;
            //        db.Entry(_page).Property(x => x.F_MenuID).IsModified = false;
            //        db.Entry(_page).Property(x => x.isDeleted).IsModified = false;
            //        db.Entry(_page).Property(x => x.Lang).IsModified = false;
            //        db.Entry(_page).Property(x => x.PageType).IsModified = false;
            //        await db.SaveChangesAsync();
            //        return Ok();
            //    }
            //}
            //else
            //{
               
            //        Pages addpage = Mapper.Map<PageDataModel, Pages>(p);
            //        addpage.F_UserID = Tools.UserID();
            //        addpage.F_MenuID = menuid;
            //        addpage.isDeleted = false;
            //    addpage.Lang = menu.Language;
            //    addpage.PageType = "UserPage";
            //        db.Pages.Add(addpage);
            //        await db.SaveChangesAsync();
            //            return Ok();
           
            //}  
            //return BadRequest();
        }
        [Route("api/Page/getPages")]
        [ResponseType(typeof(List<PageDataModel>))]
        public IHttpActionResult getPages()
        {
            string userid = Tools.RootUserID();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var page = db.Menu.Where(x => x.F_UserID == userid && x.isDeleted == false  && x.Type == "StaticPost" && (string.IsNullOrEmpty(x.ActionContent) ||  x.ActionContent.EndsWith(".html"))).ToList();
            List<PageDataModel> _list = new List<PageDataModel>();
            foreach (var item in page)
            {
                _list.Add(Mapper.Map<Menu, PageDataModel>(item));
            }
            return Ok(_list);
        }


        /// <summary>
        /// جزییات پیج مشخص شده
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns></returns>
        [Route("api/Page/getPage")]
        [ResponseType(typeof(PageDataModel))]
        public async Task<IHttpActionResult> getPage(int menuid)
        {
            string userid = Tools.RootUserID();
            var page =await db.Menu.Where(x => x.F_UserID == userid && x.ID==menuid && x.isDeleted==false &&  x.Type == "StaticPost" && (string.IsNullOrEmpty(x.ActionContent) || x.ActionContent.EndsWith(".html"))).FirstOrDefaultAsync();

            return Ok(Mapper.Map<Menu, PageDataModel>(page));
        }
        [AllowAnonymous]
        [Route("api/Page/getPageUser")]
        [ResponseType(typeof(PageDataModel))]
        public async Task<IHttpActionResult> getPageUser(int menuid,string username,string lang)
        {
            string userid = Tools.UserID(username);

            var page =await db.Menu.Where(x => x.F_UserID == userid  &&  ( (menuid==0 && x.ActionContent.StartsWith("Index")) || x.ID == menuid ) && x.isDeleted == false && x.Type == "StaticPost" && (string.IsNullOrEmpty(lang) | x.Language==lang)).FirstOrDefaultAsync();
       
            return Ok(Mapper.Map<Menu, PageDataModel>(page));
        }
        [Route("api/Page/DeletePage")]
        [ResponseType(typeof(Posts))]
        public async Task<IHttpActionResult> DeletePage(int id)
        {
            string userid = Tools.RootUserID();
            if (!PageExists(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var page = await db.Menu.Where(u=>u.ID==id && u.F_UserID==userid && u.isDeleted==false && u.Type=="StaticPost" && (string.IsNullOrEmpty(u.ActionContent) || u.ActionContent.EndsWith(".html"))).FirstOrDefaultAsync();
            if (page.F_UserID == userid)
            {
                page.ActionContent = null;


                await db.SaveChangesAsync();

                return Ok();
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        private bool PageExists(int id)
        {
            string userId = Tools.RootUserID();
            return db.Menu.Count(e => e.ID == id && e.F_UserID == userId) > 0;
        }
    }
}