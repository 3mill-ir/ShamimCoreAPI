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
using System.Web.Http.Description;

namespace CoreAPI.Controllers
{
    public class ItemGalleryController:ApiController
    {
        Entities db = DBConnect.getConnection();

        [Route("api/ItemGallery/PostItemGallery")]
        [ResponseType(typeof(ItemGalleyDataModel))]
        public async Task<IHttpActionResult>PostItemGallery(ItemGalleyListDataModel itemgallery)
        {
            string userid = Tools.UserID();

            var Item = db.Item.Where(u => u.ID == itemgallery.F_ItemID && u.F_UserID == userid).FirstOrDefault();//security checking
            if(Item==null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            ItemGallery _gallery;
            foreach (var gal in itemgallery.itemGallery)
            {
                _gallery = new ItemGallery();
                _gallery.FileName = gal.FileName;
                _gallery.F_ItemID = itemgallery.F_ItemID;
                db.ItemGallery.Add(_gallery);
            }

            await db.SaveChangesAsync();
            return Ok();
        }

        [Route("api/ItemGallery/GetItemGallery")]
        [ResponseType(typeof(ItemGalleyDataModel))]
        public IHttpActionResult GetItemGallery(int Itemid)//item id
        {
            ItemGalleyListDataModel _itemgallery = new ItemGalleyListDataModel();
            var itemgallery = db.ItemGallery.Where(u => u.F_ItemID == Itemid).ToList();
            ItemGalleyDataModel gal;
            foreach (var item in itemgallery)
            {
                gal = new ItemGalleyDataModel ();
                gal.FileName = item.FileName;
                gal.ID = item.ID;
                _itemgallery.itemGallery.Add(gal);
   
            }
            _itemgallery.F_ItemID = Itemid;
  
            return Ok(_itemgallery);
        }
        [Route("api/ItemGallery/DeleteItemGallery")]
        [ResponseType(typeof(ItemGalleyDataModel))]
        public async Task<IHttpActionResult> DeleteItemGallery(int id)
        {
            string userid = Tools.UserID();
            if(!AExists(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var itemgallery = await db.ItemGallery.FindAsync(id);
            var item = db.Item.Where(u => u.ID == itemgallery.F_ItemID && u.F_UserID==userid).FirstOrDefault();
            if(item==null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);

            }

            db.ItemGallery.Remove(itemgallery);
            await db.SaveChangesAsync();
            return Ok();
        }

        private bool AExists(int id)
        {

            return db.ItemGallery.Count(e => e.ID == id) > 0;
        }

    }
}