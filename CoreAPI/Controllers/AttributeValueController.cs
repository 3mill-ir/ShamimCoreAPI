using AutoMapper;
using CoreAPI.Models;
using CoreAPI.Models.BLL;
using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CoreAPI.Controllers
{
    public class AttributeValueController:ApiController
    {
        Entities db = DBConnect.getConnection();
        public AttributeValueController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AttributeValue, AttributeValueDataModel>();
                cfg.CreateMap<AttributeValueDataModel, AttributeValue>();
            });
        }
        [Route("api/AttributeValue/PostAttributeValue")]
        public async Task<IHttpActionResult> PostAttributeValue(AttributeValueDataModel attribute)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(IsAuthorized(attribute.F_ItemID??default(int)))
            {
                return Content(HttpStatusCode.Forbidden, "غیرمجاز");
            }
            AttributeValue _attribute = Mapper.Map<AttributeValueDataModel, AttributeValue>(attribute);
            db.AttributeValue.Add(_attribute);
            await db.SaveChangesAsync();
            return Ok("با موفقیت ثبت شد");
        }
        public bool IsAuthorized(int id)
        {
            /*  var role = Tools.UserRole();
           if (role == "admin" || role == "Company")
               return true;*/
            string userid = Tools.UserID();
            return db.Item.Count(u => u.ID == id && u.F_UserID == userid) > 0;
        }
    
    }
   
}