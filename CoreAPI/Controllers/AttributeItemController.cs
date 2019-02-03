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
    public class AttributeItemController:ApiController
    {
        Entities db = DBConnect.getConnection();
        public AttributeItemController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AttributeItem, AttributeItemDataModel>();
                cfg.CreateMap<AttributeItemDataModel, AttributeItem>();
                cfg.CreateMap<Models.DataModel.Attribute, AttributeDataModel>();
                cfg.CreateMap<AttributeDataModel, Models.DataModel.Attribute>();

            });
        }

        /// <summary>
        /// افزودن
        /// attributeitem
        /// جدید به یک
        /// attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Route("api/AttributeItem/PostAttributeItem")]
        [ResponseType(typeof(AttributeItemDataModel))]
        public async Task<IHttpActionResult> PostAttributeItem(AttributeItemDataModel attribute)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(!Authorized(attribute.F_AttributeID??(default(int))))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            AttributeItem _attribute = Mapper.Map<AttributeItemDataModel, AttributeItem>(attribute);
            db.AttributeItem.Add(_attribute);
            await db.SaveChangesAsync();
            return Ok("با موفقیت ثبت شد");
        }

        /// <summary>
        /// جزییات یک
        /// attributeitem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/AttributeItem/GetAttributeItemDetails")]
        [ResponseType(typeof(AttributeItemDataModel))]
        public async Task<IHttpActionResult> GetAttributeItemDetails(int id)
        {
            if (!AttributeItemExists(id))
            {
                return Content(HttpStatusCode.NotFound, "آیتم مورد نظر یافت نشد");
            }
            var attribute = await db.AttributeItem.FindAsync(id);
            if (!Authorized(attribute.F_AttributeID ?? default(int)))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            return Ok(Mapper.Map<AttributeItem, AttributeItemDataModel>(attribute));
        }
        [Route("api/AttributeItem/GetAttributeItemAttributes")]
        [ResponseType(typeof(AttributeDataModel))]
        public IHttpActionResult GetAttributeItemAttributes(int id)//attributeItem id
        {
            if (!AttributeItemExists(id))
            {
                return Content(HttpStatusCode.NotFound, "آیتم مورد نظر یافت نشد");
            }
            var atr = FindParentAttribute(id);
            if (!Authorized(atr.ID))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var attribute = db.Attribute.Where(u => u.F_AttributeItemID == id);

            List<AttributeDataModel> _attribute = new List<AttributeDataModel>();
            foreach (var item in attribute)
            {
                _attribute.Add(Mapper.Map<Models.DataModel.Attribute, AttributeDataModel>(item));
            }
            return Ok(_attribute);
        }


        /// <summary>
        /// ویرایش یک
        /// attributeitem
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        [Route("api/AttributeItem/PutAttributeItem")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAttributeItem(AttributeItemDataModel attribute)
        {
         
            if(!AttributeItemExists(attribute.ID))
            {
                return Content(HttpStatusCode.NotFound, "آیتم مورد نظر پیدا نشد");
            }
            if(!Authorized(attribute.F_AttributeID??default(int)))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var _attribute =await db.AttributeItem.FindAsync(attribute.ID);
            Mapper.Map(attribute, _attribute);
            db.Entry(_attribute).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok("با موفقیت ویرایش شد");
        }


        /// <summary>
        /// لیست 
        /// attribteitem
        /// های یک
        /// attribute
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/AttributeItem/GetAttribute")]
        [ResponseType(typeof(List<AttributeItemDataModel>))]
        public async Task<IHttpActionResult> GetAttribute(int id)//attribute id
        {
            List<AttributeDataModel> _attrbutes = new List<AttributeDataModel>();
            if (!Authorized(id))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            var attributes = await db.Attribute.Where(u => u.F_AttributeItemID == id).ToListAsync();
            foreach (var item in attributes)
            {
                _attrbutes.Add(Mapper.Map<CoreAPI.Models.DataModel.Attribute, AttributeDataModel>(item));
            }
            return Ok(_attrbutes);
        }

        /// <summary>
        /// حذف یک 
        /// attributeitem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/AttributeItem/DeleteAttributeItem")]
        public async Task<IHttpActionResult> DeleteAttributeItem(int id)
        {
            if (!AttributeItemExists(id))
            {
                return Content(HttpStatusCode.NotFound, "آیتم مورد نظر پیدا نشد");
            }
          
            var attribute = await db.AttributeItem.FindAsync(id);
            if (!Authorized(attribute.F_AttributeID ?? default(int)))
            {
                return Content(HttpStatusCode.Forbidden, "غیر مجاز یا نقص در اطلاعات ورودی");
            }
            db.Entry(attribute).State = EntityState.Deleted;
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
        public bool AttributeItemExists(int id)
        {
            return db.AttributeItem.Count(u => u.ID==id &&u.F_AttributeID!=null) > 0;
        }
    }
}