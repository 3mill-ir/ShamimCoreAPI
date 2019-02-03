using CoreAPI.Models;
using CoreAPI.Models.BLL;
using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoreAPI.Controllers
{
    public class ShopController : ApiController
    {
        Entities db = DBConnect.getConnection();
        [Route("api/Shop/PostShop")]
        [HttpPost]
        public IHttpActionResult PostShop(string profile, List<ShopItem> model)
        {
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.AddShop(profile, model);
            return Ok(Result);
        }

        [Route("api/Shop/PutEditShop")]
        public IHttpActionResult PutEditShop(Shop model)
        {
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.EditShop(model);
            return Ok(Result);
        }

        [Route("api/Shop/PutEditShopItem")]
        public IHttpActionResult PutEditShopItem(List<ShopItem> model)
        {
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.EditShopItem(model);
            return Ok(Result);
        }

        [Route("api/Shop/PostChangeShopState")]
        [HttpPost]
        public IHttpActionResult PostChangeShopState(Shop model)
        {
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.ChangeShopState(model);
            return Ok(Result);
        }

        [Route("api/Shop/PostDeleteShop")]
        [HttpPost]
        public IHttpActionResult PostDeleteShop(int F_ShopID)
        {
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.DeleteShop(F_ShopID);
            return Ok(Result);
        }
        [Route("api/Shop/GetListShop")]
        public IHttpActionResult GetListShop(string profile,string ShopStatus, int pageNumber, int pageSize)
        {
            string F_UserID = Tools.UserID();
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.ListShop(F_UserID, ShopStatus, pageNumber, pageSize,profile);
            return Ok(Result);
        }

        [Route("api/Shop/GetListShopForAdmin")]
        public IHttpActionResult GetListShopForAdmin(string ShopStatus, int pageNumber, int pageSize, bool IsAdmin = false)
        {
            string F_UserID = Tools.UserID();
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.ListShopForAdmin(ShopStatus, pageNumber, pageSize);
            return Ok(Result);
        }
        [Route("api/Shop/GetShopDetail")]
        public IHttpActionResult GetShopDetail(int ShopID)
        {
            ShopManagement sm = new ShopManagement(db);
            var Result = sm.ShopDetail(ShopID);
            return Ok(Result);
        }
    }
}
