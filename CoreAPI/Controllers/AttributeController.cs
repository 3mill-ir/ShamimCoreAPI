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
    public class AttributeController : ApiController
    {
        Entities db = DBConnect.getConnection();
        public AttributeController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Models.DataModel.Attribute, AttributeDataModel>();
                cfg.CreateMap<AttributeDataModel, Models.DataModel.Attribute>();
                cfg.CreateMap<AttributeItem, AttributeItemDataModel>();
                cfg.CreateMap<AttributeItemDataModel, AttributeItem>();
            });
        }


        /// <summary>
        /// افزودن
        /// attribute 
        /// جدید
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Route("api/Attribute/PostAttribute")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostAttribute(AttributeDataModel attribute)
        {
            Models.DataModel.Attribute _attribute = Mapper.Map<AttributeDataModel, Models.DataModel.Attribute>(attribute);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            int gid = _attribute.F_AttributeGroupID ?? default(int);
            if (attribute.F_AttributeGroupID == null && attribute.F_AttributeItemID != null)
            {
                gid = FindParentAttribute(attribute.F_AttributeItemID??default(int)).F_AttributeGroupID??default(int);
            }
            if (!AttributeGroupExists(gid))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            if (_attribute.F_AttributeItemID != null)
            {
                _attribute.F_AttributeGroupID = null;
            }
            db.Attribute.Add(_attribute);
            await db.SaveChangesAsync();
            return Ok();
        }


        /// <summary>
        /// جزییات یک 
        /// attribute
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Attribute/GetAttributeDetail")]
        [ResponseType(typeof(AttributeDataModel))]
        public IHttpActionResult GetAttributeDetail(int id)
        {
            AttributeDataModel _attribute = new AttributeDataModel();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!Authorized(id))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var attribute = db.Attribute.Find(id);
            _attribute = Mapper.Map<Models.DataModel.Attribute, AttributeDataModel>(attribute);
            return Ok(_attribute);
        }


        /// <summary>
        /// لیست 
        /// attribteitem
        /// های یک
        /// attribute
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Attribute/GetAttributeItem")]
        [ResponseType(typeof(List<AttributeItemDataModel>))]
        public async Task<IHttpActionResult> GetAttributeItem(int id)//attribute id
        {
            List<AttributeItemDataModel> _attrbutes = new List<AttributeItemDataModel>();
            if (!Authorized(id))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var attributes = await db.AttributeItem.Where(u => u.F_AttributeID == id).ToListAsync<AttributeItem>();
            foreach (var item in attributes)
            {
                _attrbutes.Add(Mapper.Map<AttributeItem, AttributeItemDataModel>(item));
            }
            return Ok(_attrbutes);
        }
        /*       [Route("api/Attribute/GetAllAttributes")]
               public async Task<IHttpActionResult> GetAllAttributes(int menuid)
               {
                   var attributegroups =await db.AttributeGroup.Where(u => u.F_MenuID == menuid).ToListAsync();
                   List<Models.DataModel.Attribute> attributes = new List<Models.DataModel.Attribute>();
                   List<AttributeDataModel> _attributes = new List<AttributeDataModel>();
                   foreach (var item in attributegroups)
                   {
                       var attribute = await db.Attribute.Where(u => u.F_AttributeGroupID == item.ID).ToListAsync();
                       attributes.AddRange(attribute);
                   }
                   foreach (var item in attributes)
                   {
                       _attributes.Add(Mapper.Map<Models.DataModel.Attribute, AttributeDataModel>(item));
                   }
                   return Ok(_attributes);
               }*/
        [Route("api/Attribute/GetAttributeType")]
        public async Task<IHttpActionResult> GetAttributeType(int id, string type)// attributegroup id
        {
            var attributes = await db.Attribute.Where(u => u.F_AttributeGroupID == id && u.ComponentType == type).ToListAsync<Models.DataModel.Attribute>();
            if (!Authorized(attributes.FirstOrDefault().ID))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            List<AttributeDataModel> _attributes = new List<AttributeDataModel>();
            foreach (var item in attributes)
            {
                _attributes.Add(Mapper.Map<Models.DataModel.Attribute, AttributeDataModel>(item));
            }
            return Ok(_attributes);
        }


        /// <summary>
        /// ویرایس
        /// attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Route("api/Attribute/PutAttribute")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAttribute(AttributeDataModel attribute)
        {
        
            if (!AttributeExists(attribute.ID))
            {
                return Content(HttpStatusCode.NotFound, "ویژگی مورد نظر یافت نشد");
            }
            if (!Authorized(attribute.ID))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var _attribute = db.Attribute.Find(attribute.ID);
            Mapper.Map(attribute, _attribute);
            db.Entry(_attribute).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok("با موفقیت ویرایش شد");
        }

        /// <summary>
        /// حذف آبشاری یک 
        /// attribute
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Attribute/DeleteAttribute")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteAttribute(int id)
        {
            if (!AttributeExists(id))
            {
                return Content(HttpStatusCode.NotFound, "ویژگی مورد نظر یافت نشد");
            }
            var _attribute = db.Attribute.Find(id);
            if (!Authorized(id))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var attributeitems = db.AttributeItem.Where(u => u.F_AttributeID == id);
            var attributeval = db.AttributeValue.Where(u => u.F_AttributeID == id);
            db.Attribute.Remove(_attribute);
            foreach (var item in attributeitems)
            {
                db.AttributeItem.Remove(item);
            }
            foreach (var item in attributeval)
            {
                db.AttributeValue.Remove(item);
            }
            await db.SaveChangesAsync();
            return Ok("با موفقیت حذف شد");
        }

        public bool Authorized(int id)//attribute id
        {
            /*  var role = Tools.UserRole();
           if (role == "admin" || role == "Company")
               return true;*/
            string userid = Tools.UserID();
            Models.DataModel.Attribute _attribute = db.Attribute.Find(id);
            if (_attribute != null)
            {
                if (_attribute.F_AttributeItemID != null && _attribute.F_AttributeGroupID == null)
                {
                    _attribute = FindParentAttribute(_attribute.F_AttributeItemID ?? default(int));
                }
                var attributegroup = db.AttributeGroup.Find(_attribute.F_AttributeGroupID);
                if (attributegroup != null)
                {

                    if (attributegroup.F_AttributeGroupID != null && attributegroup.F_MenuID == null)
                    {
                        attributegroup = FindParent(attributegroup.ID);
                    }
                    var uid = db.Menu.Where(u => u.ID == attributegroup.F_MenuID).FirstOrDefault().F_UserID;
                    return uid == userid ? true : false;
                }
              
            }   //db.Entry(attributegroup).State = EntityState.Detached;
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
        private Models.DataModel.Attribute FindParentAttribute(int itemid)
        {
            var attributeitem = db.AttributeItem.Find(itemid);
            return db.Attribute.Find(attributeitem.F_AttributeID);
        }
        public bool AttributeExists(int id)
        {
            return db.Attribute.Count(e => e.ID == id) > 0;
        }
        private bool AttributeGroupExists(int id)
        {
          /*  var role = Tools.UserRole();
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
    }
}