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
    public class AttributeGroupController : ApiController
    {
        Entities db = DBConnect.getConnection();
        public AttributeGroupController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AttributeGroup, AttributeGroupDataModel>();
                cfg.CreateMap<AttributeGroupDataModel, AttributeGroup>();
                cfg.CreateMap<Models.DataModel.Attribute, AttributeDataModel>();
                cfg.CreateMap<AttributeDataModel, Models.DataModel.Attribute>();
                cfg.CreateMap<AttributeItem, AttributeItemDataModel>();
                cfg.CreateMap<AttributeItemDataModel, AttributeItem>();


                cfg.CreateMap<AttributeGroup, AttributeGroupNew>().ForMember(x => x.Attribute, opt => opt.Ignore());
                cfg.CreateMap<AttributeGroupNew, AttributeGroup>().ForMember(x => x.Attribute, opt => opt.Ignore());
            });
        }


        /// <summary>
        /// افزودن
        /// attributegroup 
        /// جدید
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Route("api/AttributeGroup/PostAttributeGroup")]
        [ResponseType(typeof(AttributeGroupDataModel))]
        public IHttpActionResult PostAttributeGroup(AttributeGroupDataModel attribute)
        {
            var userid = Tools.UserID();
            AttributeGroup _attribute = Mapper.Map<AttributeGroupDataModel, AttributeGroup>(attribute);
            var menu = db.Menu.Where(u => u.ID == attribute.F_MenuID && u.F_UserID == userid).FirstOrDefault();

            if ((menu == null) && attribute.F_AttributeGroupID == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (_attribute.F_AttributeGroupID != null)
            {
                _attribute.F_MenuID = null;
            }
            db.AttributeGroup.Add(_attribute);
            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// لیست 
        /// attributegroup
        /// های یک منو یا دسته بندی
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns>OK</returns>
        [Route("api/AttributeGroup/GetAttributeGroups")]
        [Authorize(Roles = "Fanbazar")]
        [ResponseType(typeof(List<AttributeGroupNew>))]
        public IHttpActionResult GetAttributeGroups(int menuid)
        {
            var ldb = DBConnect.getEnabledLazyConnection();



            string userid = Tools.UserID();
            List<AttributeGroupNew> _attribute = new List<AttributeGroupNew>();
            //zarza change
            var menu = ldb.Menu.AsNoTracking().Where(u => u.ID == menuid && u.F_UserID == userid);

            foreach (var m in menu)
            {
                foreach (var item in m.AttributeGroup)
                {
                    _attribute.Add(Mapper.Map<AttributeGroup, AttributeGroupNew>(item));
                }
            }


            return Ok(_attribute);
        }


        /// <summary>
        /// جزییات یک
        /// attributegroup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/AttributeGroup/GetAttributeGroupDetails")]
        [ResponseType(typeof(AttributeGroupDataModel))]
        public async Task<IHttpActionResult> GetAttributeGroupDetails(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!AttributeGroupExists(id))
            {
                return Content(HttpStatusCode.NotFound, "ویژگی مورد نظر یافت نشد");
            }
            var attribute = await db.AttributeGroup.FindAsync(id);
            AttributeGroupDataModel _attribute = Mapper.Map<AttributeGroup, AttributeGroupDataModel>(attribute);
            return Ok(_attribute);
        }

        /// <summary>
        /// لیست 
        /// attribute 
        /// های یک
        /// Attributegroup 
        /// فن بازار
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/AttributeGroup/GetAttributeGroupAttributes")]
        [ResponseType(typeof(List<AttributeDataModel>))]
        public async Task<IHttpActionResult> GetAttributeGroupAttributes(int id)
        {
            List<AttributeDataModel> _attributes = new List<AttributeDataModel>();
            if (!AttributeGroupExists(id))
            {
                return Content(HttpStatusCode.NotFound, "ویژگی مورد نظر یافت نشد");
            }
            var attributes = await db.Attribute.Where(u => u.F_AttributeGroupID == id).ToListAsync<Models.DataModel.Attribute>();
            foreach (var item in attributes)
            {
                _attributes.Add(Mapper.Map<Models.DataModel.Attribute, AttributeDataModel>(item));
            }
            return Ok(_attributes);
        }


        /// <summary>
        /// ویرایش یک
        /// attributegroup
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Route("api/AttributeGroup/PutAttributeGroup")]
        [Authorize(Roles = "Fanbazar")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAttributeGroup(AttributeGroupNew attribute)
        {

            string userId = Tools.UserID();
            var _attribute = db.AttributeGroup.Find(attribute.ID);
            if (_attribute == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);

            }

            int f_menuid = FindParent(_attribute.ID).F_MenuID ?? default(int);
            if (f_menuid == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (db.Menu.Any(u => u.ID == f_menuid && u.F_UserID == userId) == false)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            Mapper.Map(attribute, _attribute);
            db.Entry(_attribute).State = EntityState.Modified;
            db.Entry(_attribute).Property(u => u.F_MenuID).IsModified = false;
            db.Entry(_attribute).Property(u => u.F_AttributeGroupID).IsModified = false;
            //db.Entry(_attribute).Property(u => u.AttributeGroup1).IsModified = false;
            //db.Entry(_attribute).Property(u => u.AttributeGroup2).IsModified = false;
            //db.Entry(_attribute).Property(u => u.Attribute).IsModified = false;
            db.SaveChanges();
            // db.Entry(_attribute).Reload();
            //foreach (var entity in db.ChangeTracker.Entries())
            //{
            //    entity.Reload();
            //}


            return Ok();


        }


        /// <summary>
        /// حذف آبشاری یک
        /// attributegroup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/AttributeGroup/DeleteAttributeGroup")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteAttributeGroup(int id)
        {
            Entities ldb = DBConnect.getEnabledLazyConnection();

            if (!AttributeGroupExists(id))
            {
                return Content(HttpStatusCode.NotFound, "ویژگی مورد نظر پیدا نشد");
            }
            CascadeTools ct = new CascadeTools();


            var _attribute = ldb.AttributeGroup.Find(id);
            ct.DeleteCascadeAttributeGroup(_attribute, ldb);
            /* var attrs = db.Attribute.Where(u => u.F_AttributeGroupID == id);
             db.AttributeGroup.Remove(_attribute);
             foreach (var item in attrs)
             {
                 db.Attribute.Remove(item);
             }*/
            if (ldb.SaveChanges() > 0)
                return Ok("ویژگی مورد نظر با موفقیت حذف شد");
            else
                return Content(HttpStatusCode.InternalServerError, "خطا در عملیات");
        }
        private bool AttributeGroupExists(int id)
        {
            /* var role = Tools.UserRole();
             if (role == "admin" || role == "Company")
                 return true;*/
            var atr = db.AttributeGroup.Find(id);
            string userid = Tools.UserID();
            if (atr != null)
            {
                if (atr.F_AttributeGroupID != null)
                {
                    atr = FindParent(id);
                }
                var menu = db.Menu.Where(u => u.ID == atr.F_MenuID && u.F_UserID == userid).FirstOrDefault();
                if (menu != null)
                {
                    db.Entry(atr).State = EntityState.Detached;
                    return true;

                }
            }
            return false;
        }
        private AttributeGroup FindParent(int id)
        {
            var attribute = db.AttributeGroup.Find(id);
            if (attribute.F_MenuID == null)
            {
                return FindParent(attribute.F_AttributeGroupID ?? default(int));
            }
            else
            {
                return attribute;
            }
        }
    }
}