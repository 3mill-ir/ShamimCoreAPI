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
using System.Linq.Dynamic;
using System.Collections.ObjectModel;

namespace CoreAPI.Controllers
{
    public class ItemController : ApiController
    {
        Entities db = DBConnect.getConnection();
        public ItemController()
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<Item, ItemDataModel>();
                //cfg.CreateMap<ItemDataModel, Item>();
                //cfg.CreateMap<AttributeGroup, AttributeGroupDataModel>();
                //cfg.CreateMap<AttributeGroupDataModel, AttributeGroup>();
                //cfg.CreateMap<Models.DataModel.Attribute, AttributeDataModel>();
                //cfg.CreateMap<AttributeDataModel, Models.DataModel.Attribute>();
                //cfg.CreateMap<AttributeItem, AttributeItemDataModel>();
                //cfg.CreateMap<AttributeItemDataModel, AttributeItem>();
                //cfg.CreateMap<AttributeValueDataModel, AttributeValue>();
                //cfg.CreateMap<AttributeValue, AttributeValueDataModel>();
                //cfg.CreateMap<ItemGallery, ItemGalleyDataModel>();
                //cfg.CreateMap<ItemGalleyDataModel, ItemGallery>();

                cfg.CreateMap<Item, ItemNew>();
                cfg.CreateMap<ItemNew, Item>();
                cfg.CreateMap<Menu, MenuNew>();
                cfg.CreateMap<MenuNew, Menu>();
                cfg.CreateMap<AttributeGroup, AttributeGroupNew>();
                cfg.CreateMap<AttributeGroupNew, AttributeGroup>();
                cfg.CreateMap<Models.DataModel.Attribute, AttributeNew>();
                cfg.CreateMap<AttributeNew, Models.DataModel.Attribute>();
                cfg.CreateMap<AttributeValue, AttributeValueNew>();
                cfg.CreateMap<AttributeValueNew, AttributeValue>();
                cfg.CreateMap<AttributeItem, AttributeItemNew>();
                cfg.CreateMap<AttributeItemNew, AttributeItem>();
            });
        }



        /// <summary>
        ///لیست ایتم ها بر اساس  
        /// itemfilterdatamodel
        /// </summary>
        /// <param name="itemfilterdatamodel"></param>
        /// <returns></returns>
        [Authorize(Roles = "Fanbazar")]
        [Route("api/Item/PostListItemsForFanbazarAdmin")]
        [ResponseType(typeof(List<ItemNew>))]
        public IHttpActionResult PostListItemsForFanbazarAdmin(FilterItemDataModel itemfilterdatamodel)
        {
            List<ItemNew> _items = new List<ItemNew>();
            string userid = Tools.UserID();
            var items = db.Item.AsNoTracking().Where(u => u.Menu.F_UserID == userid && u.Menu.isDeleted == false &&
              (itemfilterdatamodel.MenuId == 0 | u.F_MenuID == itemfilterdatamodel.MenuId) &&
               (string.IsNullOrEmpty(itemfilterdatamodel.type) | itemfilterdatamodel.type.Contains(u.Type)) &&
              (itemfilterdatamodel.FromTime == null | u.CreatedDateOnUTC >= itemfilterdatamodel.FromTime) &&
              (itemfilterdatamodel.ToTime == null | u.CreatedDateOnUTC <= itemfilterdatamodel.ToTime) &&
             (string.IsNullOrEmpty(itemfilterdatamodel.search) |
             (itemfilterdatamodel.Searchtype == 1 && u.Name.Contains(itemfilterdatamodel.search)) ||
             (itemfilterdatamodel.Searchtype == 2 && (u.Name.Contains(itemfilterdatamodel.search) || u.Description.Contains(itemfilterdatamodel.search)))
             )
              );

            string sort_param = "";
            switch (itemfilterdatamodel.sortby)
            {
                case "DateNew":
                    sort_param = "CreatedDateOnUTC Descending";
                    break;
                case "DateOld":
                    sort_param = "CreatedDateOnUTC Ascending";
                    break;
                case "NumberOfVisitors":
                    sort_param = "NumberOfVisitors Descending";
                    break;
                default:
                    sort_param = "ID Descending";
                    break;
            }


            var tempLastPostList = items.OrderBy(sort_param).Skip((itemfilterdatamodel.PageNumber - 1) * itemfilterdatamodel.PageSize).Take(itemfilterdatamodel.PageSize).ToList();

            foreach (var itm in tempLastPostList)
            {
                _items.Add(Mapper.Map<Item, ItemNew>(itm));
            }
            return Ok(_items);
        }

        /// <summary>
        ///لیست ایتم ها بر اساس  
        /// itemfilterdatamodel
        /// </summary>
        /// <param name="itemfilterdatamodel"></param>
        /// <returns></returns>
        [Authorize(Roles = "FanbazarUser")]
        [Route("api/Item/PostListItemsForFanbazarUser")]
        [ResponseType(typeof(List<ItemNew>))]
        public IHttpActionResult PostListItemsForFanbazarUser(FilterItemDataModel itemfilterdatamodel)
        {
            List<ItemNew> _items = new List<ItemNew>();
            string userid = Tools.UserID();
            var items = db.Item.AsNoTracking().Where(u => u.F_UserID == userid && u.Menu.isDeleted == false && u.Menu.Status == true &&
              (itemfilterdatamodel.MenuId == 0 | u.F_MenuID == itemfilterdatamodel.MenuId) &&
               (string.IsNullOrEmpty(itemfilterdatamodel.type) | itemfilterdatamodel.type.Contains(u.Type)) &&
              (itemfilterdatamodel.FromTime == null | u.CreatedDateOnUTC >= itemfilterdatamodel.FromTime) &&
              (itemfilterdatamodel.ToTime == null | u.CreatedDateOnUTC <= itemfilterdatamodel.ToTime) &&
             (string.IsNullOrEmpty(itemfilterdatamodel.search) |
             (itemfilterdatamodel.Searchtype == 1 && u.Name.Contains(itemfilterdatamodel.search)) ||
             (itemfilterdatamodel.Searchtype == 2 && (u.Name.Contains(itemfilterdatamodel.search) || u.Description.Contains(itemfilterdatamodel.search)))
             )
              );

            string sort_param = "";
            switch (itemfilterdatamodel.sortby)
            {
                case "DateNew":
                    sort_param = "CreatedDateOnUTC Descending";
                    break;
                case "DateOld":
                    sort_param = "CreatedDateOnUTC Ascending";
                    break;
                case "NumberOfVisitors":
                    sort_param = "NumberOfVisitors Descending";
                    break;
                default:
                    sort_param = "ID Descending";
                    break;
            }


            var tempLastPostList = items.OrderBy(sort_param).Skip((itemfilterdatamodel.PageNumber - 1) * itemfilterdatamodel.PageSize).Take(itemfilterdatamodel.PageSize).ToList();

            foreach (var itm in tempLastPostList)
            {
                _items.Add(Mapper.Map<Item, ItemNew>(itm));
            }
            return Ok(_items);
        }


        /// <summary>
        ///  لیست ایتم ها بر اساس  
        /// type
        /// </summary>
        /// <param name="username"></param>
        /// <param name="itemfilterdatamodel"></param>
        /// <returns></returns>
        [Route("api/Item/PostListItemsForUser")]
        [ResponseType(typeof(List<ItemNew>))]
        public IHttpActionResult PostListItemsForUser(string username, FilterItemDataModel itemfilterdatamodel)
        {
            List<ItemNew> _items = new List<ItemNew>();
            string userid = Tools.UserID(username);
            var items = db.Item.AsNoTracking().Where(u => u.F_UserID == userid && u.Menu.isDeleted == false && u.Menu.Status == true && u.SubmitedState == "Accepted" &&
              (itemfilterdatamodel.MenuId == 0 | u.F_MenuID == itemfilterdatamodel.MenuId) &&
               (string.IsNullOrEmpty(itemfilterdatamodel.type) | itemfilterdatamodel.type.Contains(u.Type)) &&
              (itemfilterdatamodel.FromTime == null | u.CreatedDateOnUTC >= itemfilterdatamodel.FromTime) &&
              (itemfilterdatamodel.ToTime == null | u.CreatedDateOnUTC <= itemfilterdatamodel.ToTime) &&
             (string.IsNullOrEmpty(itemfilterdatamodel.search) |
             (itemfilterdatamodel.Searchtype == 1 && u.Name.Contains(itemfilterdatamodel.search)) ||
             (itemfilterdatamodel.Searchtype == 2 && (u.Name.Contains(itemfilterdatamodel.search) || u.Description.Contains(itemfilterdatamodel.search)))
             )
              );

            string sort_param = "";
            switch (itemfilterdatamodel.sortby)
            {
                case "DateNew":
                    sort_param = "CreatedDateOnUTC Descending";
                    break;
                case "DateOld":
                    sort_param = "CreatedDateOnUTC Ascending";
                    break;
                case "NumberOfVisitors":
                    sort_param = "NumberOfVisitors Descending";
                    break;
                default:
                    sort_param = "ID Descending";
                    break;
            }


            var tempLastPostList = items.OrderBy(sort_param).Skip((itemfilterdatamodel.PageNumber - 1) * itemfilterdatamodel.PageSize).Take(itemfilterdatamodel.PageSize).ToList();

            foreach (var itm in tempLastPostList)
            {
                _items.Add(Mapper.Map<Item, ItemNew>(itm));
            }
            return Ok(_items);
        }

        /// <summary>
        /// جزییات ایتم
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Fanbazar")]
        [Route("api/Item/GetItemDetailsForFanbazarAdmin")]
        [ResponseType(typeof(ItemNew))]
        public IHttpActionResult GetItemDetailsForFanbazarAdmin(int id)//just details of item
        {
            string userId = Tools.UserID();
            var ldb = DBConnect.getEnabledLazyConnection();
            var pp = ldb.Menu.AsNoTracking().Include(u => u.Item).Include(u => u.AttributeGroup).Where(u => u.Item.Any(m => m.ID == id) && u.F_UserID == userId).FirstOrDefault();
            if (pp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            //ItemNew item = new ItemNew();
            //item = Mapper.Map<Item, ItemNew>(pp.Item.FirstOrDefault(u => u.ID == id));


            return Ok(Merge_Item(pp.Item.FirstOrDefault(u => u.ID == id), id));
        }


        /// <summary>
        /// جزییات ایتم
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "FanbazarUser")]
        [Route("api/Item/GetItemDetailsForFanbazarUser")]
        [ResponseType(typeof(ItemNew))]
        public IHttpActionResult GetItemDetailsForFanbazarUser(int id)//just details of item
        {
            string userId = Tools.UserID();
            var ldb = DBConnect.getEnabledLazyConnection();
            //var pp = ldb.Menu.AsNoTracking().Include(u => u.Item).Include(u => u.AttributeGroup).Where(u => u.Item.Any(m => m.ID == id) && u.F_UserID == userId).FirstOrDefault();
            var pp = ldb.Item.AsNoTracking().Where(u => u.ID == id && u.F_UserID == userId).FirstOrDefault();
            if (pp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            //ItemNew item = new ItemNew();
            //item = Mapper.Map<Item, ItemNew>(pp.Item.FirstOrDefault(u => u.ID == id));
            return Ok(Merge_Item(pp, id));
        }



        /// <summary>
        /// جزییات آیتم بر اساس نوع و آیدی یوزر
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Item/GetItemDetailsByType")]
        [Authorize]
        [ResponseType(typeof(ItemNew))]
        public IHttpActionResult GetItemDetailsByType(string Type, string username)//just details of item
        {
            string UserID = Tools.UserID();
            var ldb = DBConnect.getEnabledLazyConnection();
            var pp = ldb.Item.AsNoTracking().Where(m => m.Description == Type && m.F_UserID == UserID).FirstOrDefault();
            if (pp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok(Merge_Item(pp, pp.ID));
        }


        /// <summary>
        /// جزییات آیتم بر اساس نوع و آیدی یوزر
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [Route("api/Item/GetItemDetailsByTypeForUser")]
        [ResponseType(typeof(ItemNew))]
        public IHttpActionResult GetItemDetailsByTypeForUser(string Type, string UserID)//just details of item
        {
            var ldb = DBConnect.getEnabledLazyConnection();
            var pp = ldb.Item.AsNoTracking().Where(m => m.Description == Type && m.F_UserID == UserID).FirstOrDefault();
            if (pp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok(Merge_Item(pp, pp.ID));
        }


        /// <summary>
        /// جزییات آیتم
        /// </summary>
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Item/GetItemDetailsForUser")]
        [ResponseType(typeof(ItemNew))]
        public async Task<IHttpActionResult> GetItemDetailsForUser(string username, int id)//just details of item
        {
            string userId = Tools.UserID(username);
            var ldb = DBConnect.getEnabledLazyConnection();
            var pp = await ldb.Menu.AsNoTracking().Include(u => u.Item).Include(u => u.AttributeGroup).Where(u => u.Item.Any(m => m.ID == id) && u.F_UserID == userId).FirstOrDefaultAsync();
            if (pp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var _it = pp.Item.FirstOrDefault(u => u.ID == id);
            var temp = db.Item.FirstOrDefault(u => u.ID == id).NumberOfVisitors++;
            db.SaveChanges();
            //ItemNew item = new ItemNew();
            //item = Mapper.Map<Item, ItemNew>(_it);
            return Ok(Merge_Item(_it, id));
        }



        /// <summary>
        /// جزییات آیتم
        /// </summary>
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <param name="F_MenuID"></param>
        /// <returns></returns>
        [Route("api/Item/GetItemDetailsByMenu")]
        [ResponseType(typeof(MenuNew))]
        public async Task<IHttpActionResult> GetItemDetailsByMenu(string username, int id, int F_MenuID = 0)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Item, ItemNew>();
                cfg.CreateMap<ItemNew, Item>();
                cfg.CreateMap<Menu, MenuNew>().ForMember(x => x.Menu1, opt => opt.Ignore());
                cfg.CreateMap<MenuNew, Menu>().ForMember(x => x.Menu1, opt => opt.Ignore()).ForMember(x => x.Item, opt => opt.Ignore());
                cfg.CreateMap<AttributeGroup, AttributeGroupNew>();
                cfg.CreateMap<AttributeGroupNew, AttributeGroup>();
                cfg.CreateMap<Models.DataModel.Attribute, AttributeNew>();
                cfg.CreateMap<AttributeNew, Models.DataModel.Attribute>();
                cfg.CreateMap<AttributeValue, AttributeValueNew>();
                cfg.CreateMap<AttributeValueNew, AttributeValue>();
                cfg.CreateMap<AttributeItem, AttributeItemNew>();
                cfg.CreateMap<AttributeItemNew, AttributeItem>();
            });
            string userId = Tools.UserID(username);
            var ldb = DBConnect.getEnabledLazyConnection();
            var result = await ldb.Menu.Where(u => u.F_UserID == userId && u.Status == true && u.isDeleted == false && u.ID == F_MenuID).FirstOrDefaultAsync();
            foreach (var AttributeGroup in result.AttributeGroup)
            {
                foreach (var Attribute in AttributeGroup.Attribute)
                {
                    Attribute.AttributeValue = Attribute.AttributeValue.Where(u => u.F_ItemID == id).ToList();
                }
            }
            if (result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            MenuNew item = new MenuNew();
            item = Mapper.Map<Menu, MenuNew>(result);
            return Ok(item);
        }

        private ItemNew Merge_Item(Item _it, int itemID)
        {
            ItemNew _itemnew = new ItemNew();
            _itemnew.AdminDescription = _it.AdminDescription;
            _itemnew.F_UserID = _it.F_UserID;
            _itemnew.CreatedDateOnUTC = _it.CreatedDateOnUTC;
            _itemnew.Description = _it.Description;
            _itemnew.ID = _it.ID;
            _itemnew.Image = _it.Image;
            _itemnew.Name = _it.Name;
            _itemnew.Stock = _it.Stock ?? default(int);
            _itemnew.Price = _it.Price;
            _itemnew.NumberOfVisitors = _it.NumberOfVisitors;
            _itemnew.SubmitedState = _it.SubmitedState;
            _itemnew.Type = _it.Type;
            _itemnew.Menu = new MenuNew();
            _itemnew.Menu.Description = _it.Menu.Description;
            _itemnew.Menu.ID = _it.Menu.ID;
            _itemnew.Menu.Image = _it.Menu.Image;
            _itemnew.Menu.MetaDescription = _it.Menu.MetaDescription;
            _itemnew.Menu.MetaKeywords = _it.Menu.MetaKeywords;
            _itemnew.Menu.MetaSeoName = _it.Menu.MetaSeoName;
            _itemnew.Menu.MetaTittle = _it.Menu.MetaTittle;
            _itemnew.Menu.Name = _it.Menu.Name;
            _itemnew.Menu.Type = _it.Menu.Type;

            foreach (var atg in _it.Menu.AttributeGroup)
            {
                Merge_ATG(atg, _itemnew.Menu.AttributeGroup, itemID);
            }

            return _itemnew;
        }

        private void Merge_ATG(AttributeGroup ATG, Collection<AttributeGroupNew> list, int itemID)
        {
            AttributeGroupNew _item = new AttributeGroupNew();
            _item.Depth = ATG.Depth;
            _item.ID = ATG.ID;
            _item.Image = ATG.Image;
            _item.Name = ATG.Name;
            _item.Weight = ATG.Weight;
            _item.F_AttributeGroupID = ATG.F_AttributeGroupID;

            if (ATG.AttributeGroup1 != null)
            {
                foreach (var _atg in ATG.AttributeGroup1)
                {
                    Merge_ATG(_atg, _item.AttributeGroup1, itemID);
                }
            }
            Collection<AttributeNew> list_AT = new Collection<AttributeNew>();
            foreach (var AT in ATG.Attribute)
            {
                Merge_AT(AT, list_AT, itemID);
            }

            _item.Attribute = list_AT;
            list.Add(_item);
        }
        private void Merge_AT(Models.DataModel.Attribute AT, Collection<AttributeNew> list, int itemID)
        {
            AttributeNew _item = new AttributeNew();
            _item.Icon = AT.Icon;
            _item.ID = AT.ID;
            _item.Name = AT.Name;
            _item.TextColor = AT.TextColor;
            _item.Weight = AT.Weight;
            _item.ComponentType = AT.ComponentType;
            _item.F_AttributeItemID = AT.F_AttributeItemID;

            AttributeValueNew _value;
            foreach (var val in AT.AttributeValue)
            {
                if (val.F_ItemID == itemID)
                {
                    _value = new AttributeValueNew();
                    _value.Value = val.Value;
                    _value.F_AttributeID = val.F_AttributeID;
                    _value.F_AttributeItemID = val.F_AttributeItemID;
                    _item.AttributeValue.Add(_value);
                }

            }

            AttributeItemNew _ai;
            foreach (var it in AT.AttributeItem1)
            {
                _ai = new AttributeItemNew();
                _ai.ID = it.ID;
                _ai.Name = it.Name;
                _ai.F_AttributeID = it.F_AttributeID;
                Collection<AttributeNew> list_AT = new Collection<AttributeNew>();
                foreach (var _at in it.Attribute)
                {
                    Merge_AT(_at, list_AT, itemID);
                }
                _ai.Attribute = list_AT;
                _item.AttributeItem1.Add(_ai);

            }



            list.Add(_item);

        }




        /// <summary>
        /// لیست ویژگی های یک دسته
        /// </summary>
        /// <param name="username"></param>
        /// <param name="menuid"></param>
        /// <returns></returns>
        [Route("api/Item/GetAttributesForItem")]
        [ResponseType(typeof(MenuNew))]
        public async Task<IHttpActionResult> GetAttributesForItem(string username, int menuid)//
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Item, ItemNew>();
                cfg.CreateMap<ItemNew, Item>();
                cfg.CreateMap<Menu, MenuNew>().ForMember(x => x.Menu1, opt => opt.Ignore());
                cfg.CreateMap<MenuNew, Menu>().ForMember(x => x.Menu1, opt => opt.Ignore()).ForMember(x => x.Item, opt => opt.Ignore());
                cfg.CreateMap<AttributeGroup, AttributeGroupNew>();
                cfg.CreateMap<AttributeGroupNew, AttributeGroup>();
                cfg.CreateMap<Models.DataModel.Attribute, AttributeNew>();
                cfg.CreateMap<AttributeNew, Models.DataModel.Attribute>();
                cfg.CreateMap<AttributeValue, AttributeValueNew>();
                cfg.CreateMap<AttributeValueNew, AttributeValue>();
                cfg.CreateMap<AttributeItem, AttributeItemNew>();
                cfg.CreateMap<AttributeItemNew, AttributeItem>();
            });
            string userId = Tools.UserID(username);
            var ldb = DBConnect.getEnabledLazyConnection();
            var pp = await ldb.Menu.AsNoTracking().Where(u => u.ID == menuid && u.F_UserID == userId).FirstOrDefaultAsync();
            if (pp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (pp.Item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var attributes = Mapper.Map<Menu, MenuNew>(pp);
            return Ok(attributes);
        }




        /// <summary>
        /// تغییر وضعیت آیتم
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sts"></param>
        /// <returns></returns>
        [Route("api/Item/PutChangeSubmitedState")]
        //[Authorize(Roles = "Fanbazar")]
        [Authorize]
        [ResponseType(typeof(ItemDataModel))]
        public async Task<IHttpActionResult> PutChangeSubmitedState(SubmiteState sts)
        {
            string userId = Tools.UserID();
            var item = await db.Item.FirstOrDefaultAsync(u => u.ID == sts.id && u.Menu.F_UserID == userId);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            //db.Entry(item).State = EntityState.Modified;
            item.SubmitedState = sts.State;
            item.AdminDescription = sts.Description;
            db.SaveChanges();
            //db.Entry(item).State = EntityState.Detached;
            return Ok();
        }
        ///// <summary>
        ///// جزییات یک آیتم
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Route("api/Item/GetItemDetails")]
        //[ResponseType(typeof(ItemDataModel))]
        //public IHttpActionResult GetItemDetails(int id)//just details of item
        //{
        //    ItemDataModel _item = new ItemDataModel();
        //    string userid = Tools.UserID();
        //    var item = db.Item.Include(u => u.AttributeValue).Where(u => u.F_UserID == userid && u.ID == id).FirstOrDefault();
        //    _item = Mapper.Map<Item, ItemDataModel>(item);
        //    return Ok(_item);
        //}




        /// <summary>
        /// افزودن آیتم جدید 
        /// </summary>
        /// <param name="fanbazar"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Item/PostItem")]
        [Authorize(Roles = "FanbazarUser")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PostItem(ItemPostDataModel fanbazar, string username)
        {
            int id = addFanbazarItem(fanbazar, username);

            foreach (var item in fanbazar.Attributes)
            {
                addAttributeValue(item, id);
            }
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// افزودن آیتم جدید 
        /// </summary>
        /// <param name="fanbazar"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Item/PostItemForUser")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PostItemForUser(ItemPostDataModel fanbazar, string username)
        {
            int id = addFanbazarItem(fanbazar, username, true);

            foreach (var item in fanbazar.Attributes)
            {
                addAttributeValue(item, id);
            }
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// افزودن آیتم جدید 
        /// </summary>
        /// <param name="fanbazar"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Item/PutItem")]
        [Authorize(Roles = "FanbazarUser")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutItem(ItemPostDataModel fanbazar, string username)
        {
            int id = EditFanbazarItem(fanbazar, username);

            foreach (var item in fanbazar.Attributes)
            {
                EditAttributeValue(item, id);
            }
            return Ok();
        }


        /// <summary>
        /// ویرایش  یک آیتم 
        /// در صورتی که ویژگی جدید اضافه شده باشد آن را اعمال میکند
        /// </summary>
        /// <param name="fanbazar"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Item/PutItemOrInitiate")]
        [Authorize(Roles = "FanbazarUser")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutItemOrInitiate(ItemPostDataModel fanbazar, string username)
        {
            fanbazar.Attributes.RemoveAll(item => item.Value == null);
            int id = EditFanbazarItem(fanbazar, username);

            foreach (var item in fanbazar.Attributes)
            {
                EditAttributeValue(item, id, true);
            }
            db.SaveChanges();
            return Ok();
        }







        //[Route("api/Item/GetFullItemDetails")]
        //[ResponseType(typeof(FanbazarFactDataModel))]
        //public async Task<IHttpActionResult> GetFullItemDetails(int id,string type)//returns a fanbazar
        //{
        //    var ldb = DBConnect.getEnabledLazyConnection();
        //    //get the Item part of Fanbazar Data Model
        //    ItemDataModel _item = new ItemDataModel();
        //    FanbazarDataModel _fanbazar = new FanbazarDataModel();

        //    var itemmodel =await  ldb.Item.FindAsync(id);
        //    _item = Mapper.Map<Item, ItemDataModel>(itemmodel);
        //    _fanbazar.Item = _item;
        //    //getting the attributeGroup and other features
        //    var menu = db.Item.Where(u =>  u.ID == id && u.Type==type).FirstOrDefault();
        //    int menuid = menu.F_MenuID ?? default(int);
        //    List<AttributeGroupDataModel> _attribute = new List<AttributeGroupDataModel>();
        //    var attribute = ldb.AttributeGroup.Where(u => u.F_MenuID == menuid).ToList<AttributeGroup>();
        //    var attributevalue = db.AttributeValue.Where(u => u.F_ItemID == id);
        //    for (int i = 0; i < attribute.Count; i++)
        //    {
        //        AttributeGroupDataModel atr = new AttributeGroupDataModel();
        //        Mapper.Map(attribute.ElementAt(i), atr);
        //        FillAttributeGroup(ref atr, attributevalue);
        //        _attribute.Add(atr);
        //    }

        //    _fanbazar.AttributeGoups = _attribute;

        //    return Ok(_fanbazar);
        //}

        //[Route("api/Item/GetMenuItems")]
        //[ResponseType(typeof(ItemDataModel))]
        //public IHttpActionResult GetMenuItems(int id)//menu id and only returns the list of Items for current user
        //{
        //    List<ItemDataModel> _items = new List<ItemDataModel>();
        //    string userid = Tools.UserID();
        //    var items = db.Item.Where(u => u.F_UserID == userid && u.F_MenuID == id).ToList<Item>();
        //    foreach (var itm in items)
        //    {
        //        _items.Add(Mapper.Map<Item, ItemDataModel>(itm));
        //    }

        //    return Ok(_items);
        //}
        //[Route("api/Item/PostIncreaseVisitors")]
        //public async Task<IHttpActionResult> PostIncreaseVisitors(int id)//inline query ,increase numberofvisitors and returns the value of it
        //{
        //    var item = await db.Item.FindAsync(id);
        //    item.NumberOfVisitors += 1;
        //    db.Entry(item).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //    return Ok(item.NumberOfVisitors);
        //}


        [Route("api/Item/GetItemAttributes")]
        [ResponseType(typeof(FanbazarDataModel))]
        public IHttpActionResult GetItemAttributes(int menuid)//returns a Fanbazar for a category
        {
            FanbazarDataModel _fanbazar = new FanbazarDataModel();
            //  var ldb = DBConnect.getEnabledLazyConnection();
            string userid = Tools.UserID();
            List<AttributeGroupDataModel> _attributegroup = new List<AttributeGroupDataModel>();
            var uid = db.Menu.Find(menuid).F_UserID;
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (uid != userid)
            {
                return Content(HttpStatusCode.NotFound, "موردی پیدا نشد");
            }
            var attributegroup = db.AttributeGroup.AsNoTracking().Where(u => u.F_MenuID == menuid).ToList<AttributeGroup>();
            foreach (var item in attributegroup)
            {
                AttributeGroupDataModel _group = new AttributeGroupDataModel();
                List<AttributeDataModel> _atr = new List<AttributeDataModel>();
                Mapper.Map(item, _group);
                var atrs = db.Attribute.AsNoTracking().Where(u => u.F_AttributeGroupID == item.ID);
                foreach (var itm in atrs)
                {
                    _atr.Add(Mapper.Map<Models.DataModel.Attribute, AttributeDataModel>(itm));

                }
                _group.Attribute = _atr;
                _attributegroup.Add(_group);

            }
            _fanbazar.AttributeGoups = _attributegroup;
            return Ok(_fanbazar);
        }



        ///// <summary>
        ///// تغییر وضعیت یک آیتم
        ///// </summary>
        ///// <param name="post"></param>
        ///// <returns></returns>
        //[Route("api/Item/PostChangeStateDescription")]
        //[ResponseType(typeof(ItemDataModel))]
        //public async Task<IHttpActionResult> PostChangeStateDescription(PostItemDescription post)
        //{
        //    if (!AExists(post.id))
        //    {
        //        return Content(HttpStatusCode.NotFound, "محصول با آیدی مورد نظر پیدا نشد");
        //    }
        //    var item = await db.Item.FindAsync(post.id);
        //    item.SubmitedState = post.state;
        //    item.AdminDescription = post.description;
        //    await db.SaveChangesAsync();
        //    return Ok("محصول با موفقیت تغییر وضعیت داده شد");
        //}
        [Route("api/Item/DeleteItem")]
        [ResponseType(typeof(ItemDataModel))]
        public async Task<IHttpActionResult> DeleteItem(int id)
        {
            string userid = Tools.UserID();
            if (!AExists(id))
            {
                return Content(HttpStatusCode.NotFound, "محصول با آیدی مورد نظر پیدا نشد");
            }
            var item = await db.Item.FindAsync(id);
            var gitems = db.ItemGallery.AsNoTracking().Where(u => u.F_ItemID == id);
            var itemval = db.AttributeValue.AsNoTracking().Where(u => u.F_ItemID == id);
            db.Item.Remove(item);
            foreach (var itm in gitems)
            {
                db.ItemGallery.Remove(itm);
            }
            foreach (var itm in itemval)
            {
                db.AttributeValue.Remove(itm);
            }
            await db.SaveChangesAsync();
            return Ok("محصول با موفقیت حذف شد");
        }


        [Route("api/Item/PutItem")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutItem(FanbazarDataModel fanbazar)
        {

            if (!AExists(fanbazar.Item.ID))
            {
                return Content(HttpStatusCode.NotFound, "محصول با آیدی مورد نظر پیدا نشد");
            }
            string userid = Tools.UserID();
            var _item = db.Item.AsNoTracking().Where(u => u.F_UserID == userid && u.ID == fanbazar.Item.ID).FirstOrDefault();
            Mapper.Map(fanbazar.Item, _item);
            _item.F_UserID = userid;
            db.Entry(_item).State = EntityState.Modified;
            db.Entry(_item).Property(u => u.F_UserID).IsModified = false;
            db.Entry(_item).Property(u => u.Type).IsModified = false;
            foreach (var gal in _item.ItemGallery)
            {
                addItemGallery(Mapper.Map<ItemGallery, ItemGalleyDataModel>(gal), _item.ID);
            }

            var attributevalues = db.AttributeValue.Where(u => u.F_ItemID == fanbazar.Item.ID);
            foreach (var item in attributevalues)
            {
                db.AttributeValue.Remove(item);
            }
            await db.SaveChangesAsync();
            addFanbazar(fanbazar);
            return Ok("محصول با موفقیت ویرایش شد");

        }
        [Route("api/Item/PostSearchItem")]
        public IHttpActionResult PostSearchItem(ItemSearchFilter filter)
        {
            if (filter == null)
                return Ok();
            List<ItemDataModel> items = new List<ItemDataModel>();
            List<Item> lst = new List<Item>();
            if (filter.keyword != null)
            {
                lst = db.Item.AsNoTracking().Where(u => u.Description.Contains(filter.keyword) || u.Name.Contains(filter.keyword)).ToList();
            }
            else
            {
                lst = db.Item.ToList();
            }
            if (filter.type != "none" && (filter.categoryid <= 0))
            {
                lst = lst.Where(u => u.Type == filter.type).ToList(); ;

            }
            if ((filter.categoryid > 0) && (filter.type == "none"))//based on the user's request, this method must be updated 
            {
                lst = lst.Where(u => u.F_MenuID == filter.categoryid).ToList();
            }
            if ((filter.type != "none") && (filter.categoryid > 0))
            {
                lst = lst.Where(u => u.F_MenuID == filter.categoryid && u.Type == filter.type).ToList();
            }
            /* if (filter.keyword != null && items.Count > 0)
                 lst = lst.Where(u => u.Description.Contains(filter.keyword) || u.Name.Contains(filter.keyword)).ToList();
             if (filter.keyword != null && items.Count == 0)
                 lst = db.Item.Where(u => u.Description.Contains(filter.keyword) || u.Name.Contains(filter.keyword)).ToList();*/
            return Ok(lst);

        }


        [Route("api/Item/GetLatestItems")]
        public async Task<IHttpActionResult> GetLatestItems(int count, string type)//latest items of current user
        {
            string userid = Tools.UserID();
            List<ItemNew> offers = new List<ItemNew>();
            var items = await db.Item.AsNoTracking().Where(u => u.F_UserID == userid && u.Type == type).OrderByDescending(u => u.CreatedDateOnUTC).ToListAsync<Item>();
            foreach (var item in items)
            {
                offers.Add(Mapper.Map<Item, ItemNew>(item));
            }
            return Ok(offers.Take(count));
        }

        [Route("api/Item/PostListItemsForGuest")]
        [HttpPost]
        public async Task<IHttpActionResult> PostListItemsForGuest(FilterItemDataModel model)//latest items of current user
        {
            List<ItemNew> _items = new List<ItemNew>();
            string userid = Tools.UserID(model.profile);
            var items = await db.Item.AsNoTracking().Where(u => u.Menu.F_UserID == userid && u.Menu.isDeleted == false &&
               (model.MenuId == 0 | u.F_MenuID == model.MenuId) &&
                (string.IsNullOrEmpty(model.type) | model.type.Contains(u.Type)) &&
               (model.FromTime == null | u.CreatedDateOnUTC >= model.FromTime) &&
               (model.ToTime == null | u.CreatedDateOnUTC <= model.ToTime) &&
              (string.IsNullOrEmpty(model.search) |
              (model.Searchtype == 1 && u.Name.Contains(model.search)) ||
              (model.Searchtype == 2 && (u.Name.Contains(model.search) || u.Description.Contains(model.search)))
              ) && u.SubmitedState == "Accepted").ToListAsync();
            string sort_param = "";
            switch (model.sortby)
            {
                case "DateNew":
                    sort_param = "CreatedDateOnUTC Descending";
                    break;
                case "DateOld":
                    sort_param = "CreatedDateOnUTC Ascending";
                    break;
                case "NumberOfVisitors":
                    sort_param = "NumberOfVisitors Descending";
                    break;
                default:
                    sort_param = "ID Descending";
                    break;
            }

            var tempLastPostList = items.OrderBy(sort_param).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

            foreach (var itm in tempLastPostList)
            {
                _items.Add(Mapper.Map<Item, ItemNew>(itm));
            }
            return Ok(new { Items = _items, Total = items.Count });
        }

        /// <summary>
        /// لیست آخرین آیتم های ثبت شده را بر اساس
        /// type
        /// بر میگرداند
        /// </summary>
        /// <param name="username"></param>
        /// <param name="F_MenuID"></param>
        /// <param name="count"></param>
        /// <param name="type">هر یک از ایتم های زیر می تواند باشد
        /// type=FanbazarDemand,FanbazarOfferDemand
        /// یا
        ///type= FanbazarOffer,FanbazarOfferDemand
        ///یا
        /// type= FanbazarCompany</param>
        /// <returns></returns>
        [Route("api/Item/GetGeneralLatestItems")]
        [ResponseType(typeof(List<ItemDataModel>))]
        public async Task<IHttpActionResult> GetGeneralLatestItems(string username, int count, string type, int F_MenuID = 0)//returns latest items from all users
        {
            string userId = Tools.UserID(username);
            List<ItemNew> offers = new List<ItemNew>();
            var menus = F_MenuID != 0 ? (await GetMenuChild(F_MenuID)).Select(y => y.ID) : new List<int>();
            var items = db.Item.AsNoTracking().Include(u => u.Menu).Where(u => type.Contains(u.Type) && (F_MenuID == 0 | menus.Contains(u.F_MenuID ?? default(int))) && u.Menu.F_UserID == userId && u.SubmitedState == "Accepted").OrderByDescending(u => u.CreatedDateOnUTC).Take(count).ToList<Item>();
            foreach (var item in items)
            {
                offers.Add(Mapper.Map<Item, ItemNew>(item));
            }
            return Ok(offers);
        }

        public async Task<List<MenuDataModel>> GetMenuChild(int F_MenuID)
        {
            List<MenuDataModel> _list = new List<MenuDataModel>();
            var ldb = DBConnect.getEnabledLazyConnection();
            var menuList = await ldb.Menu.Where(u => u.isDeleted == false && u.Status == true && u.ID == F_MenuID).FirstOrDefaultAsync();
            _list.Add(new MenuDataModel() { Name = menuList.Name, ID = menuList.ID });
            RecursionChildFind(menuList.Menu1, _list);
            return _list;
        }

        private static void RecursionChildFind(ICollection<Menu> hierarchy, List<MenuDataModel> model)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                {
                    model.Add(new MenuDataModel() { Name = item.Name, ID = item.ID });
                    RecursionChildFind(item.Menu1, model);
                }
            }
        }

        [Route("api/Item/GetPopularItems")]
        public async Task<IHttpActionResult> GetPopularItems(int count, string type)//popular current user's items
        {
            string userid = Tools.UserID();
            List<ItemNew> offers = new List<ItemNew>();
            var items = await db.Item.AsNoTracking().Where(u => u.F_UserID == userid && u.Type == type).OrderByDescending(u => u.NumberOfVisitors).ToListAsync<Item>();
            foreach (var item in items)
            {
                offers.Add(Mapper.Map<Item, ItemNew>(item));
            }
            return Ok(offers.Take(count));
        }

        /// <summary>
        /// لیست محبوب ترین ایتم های ثبت شده را بر اساس 
        /// type
        /// بر میگرداند
        /// </summary>
        /// <param name="username"></param>
        /// <param name="count"></param>
        /// <param name="F_MenuID"></param>
        /// <param name="type">هر یک از ایتم های زیر می تواند باشد
        /// type=FanbazarDemand,FanbazarOfferDemand
        /// یا
        ///type= FanbazarOffer,FanbazarOfferDemand
        ///یا
        /// type= FanbazarCompany</param>
        /// <returns></returns>
        [Route("api/Item/GetGeneralPopularItems")]
        [ResponseType(typeof(List<ItemDataModel>))]
        public async Task<IHttpActionResult> GetGeneralPopularItems(string username, int count, string type, int F_MenuID = 0)//returns popular items of all users
        {
            string userId = Tools.UserID(username);
            List<ItemNew> offers = new List<ItemNew>();
            var menus = F_MenuID != 0 ? (await GetMenuChild(F_MenuID)).Select(y => y.ID) : new List<int>();
            var items = await db.Item.AsNoTracking().Include(u => u.Menu).Where(u => type.Contains(u.Type) && (F_MenuID == 0 | menus.Contains(u.F_MenuID ?? default(int))) && u.Menu.F_UserID == userId && u.SubmitedState == "Accepted").OrderByDescending(u => u.NumberOfVisitors).Take(count).ToListAsync<Item>();
            foreach (var item in items)
            {
                offers.Add(Mapper.Map<Item, ItemNew>(item));
            }
            return Ok(offers);
        }


        /// <summary>
        /// تعداد آیتم ها را بر اساس 
        /// type
        ///  بر میگرداند
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Item/GetCountItems")]
        [ResponseType(typeof(FanbazarFactDataModel))]
        public IHttpActionResult GetCountItems(string username)//returns popular items of all users
        {
            string userId = Tools.UserID(username);
            var count = from a in db.Item
                        where a.Menu.F_UserID == userId && a.SubmitedState == "Accepted"
                        group a by a.Type into gp
                        select new
                        {
                            count = gp.Count(),
                            type = gp.FirstOrDefault().Type
                        };
            //  var itemCount = await db.Item.Include(u => u.Menu).Where(u => type.Contains(u.Type) && u.Menu.F_UserID == userId && u.SubmitedState == "Accepted").OrderByDescending(u => u.NumberOfVisitors).CountAsync();
            FanbazarFactDataModel fact = new FanbazarFactDataModel();
            int offerdemand = count.FirstOrDefault(u => u.type == "FanbazarOfferDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOfferDemand").count;
            fact.CompanyCount = count.FirstOrDefault(u => u.type == "FanbazarCompany") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarCompany").count;
            fact.OfferCount = count.FirstOrDefault(u => u.type == "FanbazarOffer") == null ? offerdemand : count.FirstOrDefault(u => u.type == "FanbazarOffer").count + offerdemand;
            fact.DemandCount = count.FirstOrDefault(u => u.type == "FanbazarDemand") == null ? offerdemand : count.FirstOrDefault(u => u.type == "FanbazarDemand").count + offerdemand;
            fact.UserCount = db.UserInformation.Where(u => u.F_ParrentUserID == userId).Count();
            return Ok(fact);
        }







        private void addFanbazar(FanbazarDataModel fanbazar)
        {
            foreach (var item in fanbazar.AttributeGoups)
            {
                addAttributeGroup(item, fanbazar.Item.ID);
            }
            db.SaveChanges();
        }

        private void addAttributeGroup(AttributeGroupDataModel attributeGroup, int itemid)
        {
            var attributes = attributeGroup.Attribute;

            foreach (var itm in attributes)
            {
                addFanbazarAttributes(itm, itemid);
            }
            if (attributeGroup.AttributeGroup1.Count > 0)
            {
                foreach (var item in attributeGroup.AttributeGroup1)
                {
                    addAttributeGroup(item, itemid);
                }
            }
        }

        private void addAttributeValue(ItemPostHelper ItemAtt, int itemId)
        {
            if (!string.IsNullOrEmpty(ItemAtt.Value))
            {
                AttributeValue _value = new AttributeValue();
                _value.F_AttributeID = ItemAtt.F_AttributeID;
                _value.F_ItemID = itemId;
                if (ItemAtt.F_AttributeItemID == 0)
                {
                    _value.F_AttributeItemID = null;
                }
                else
                {
                    _value.F_AttributeItemID = ItemAtt.F_AttributeItemID;
                }
                _value.Value = ItemAtt.Value;

                db.AttributeValue.Add(_value);
                db.SaveChanges();
            }
        }

        private void EditAttributeValue(ItemPostHelper ItemAtt, int itemId, bool IsInitiate = false)
        {
            var _value = db.AttributeValue.FirstOrDefault(u => u.F_ItemID == itemId && u.F_AttributeID == ItemAtt.F_AttributeID);


            if (_value == null)
            {
                if (IsInitiate)
                {
                    addAttributeValue(ItemAtt, itemId);
                    _value = db.AttributeValue.FirstOrDefault(u => u.F_ItemID == itemId && u.F_AttributeID == ItemAtt.F_AttributeID);
                }
                else
                    throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (ItemAtt.F_AttributeItemID == 0)
            {
                _value.F_AttributeItemID = null;
            }
            else
            {
                _value.F_AttributeItemID = ItemAtt.F_AttributeItemID;
            }
            _value.Value = ItemAtt.Value;

            db.Entry(_value).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void addFanbazarAttributes(AttributeDataModel attr, int itemid)
        {
            if (attr.Value != null)
            {
                AttributeValueDataModel attribute = new AttributeValueDataModel();
                attribute.F_AttributeID = attr.ID;
                attribute.F_ItemID = itemid;
                attribute.Value = attr.Value;
                AttributeValue _attribute = Mapper.Map<AttributeValueDataModel, AttributeValue>(attribute);
                db.AttributeValue.Add(_attribute);

            }
            if (attr.AttributeItem1.Count > 0)
            {
                foreach (var item in attr.AttributeItem1)
                {
                    addFanbazarAttributeItem(item, itemid);
                }
            }

        }

        private void addFanbazarAttributeItem(AttributeItemDataModel attr, int itemid)
        {
            /* if (attr.Value != null)
             {*/
            AttributeValueDataModel attribute = new AttributeValueDataModel();
            attribute.F_AttributeItemID = attr.ID;
            attribute.F_ItemID = itemid;
            //  attribute.Value = attr.Value;
            AttributeValue _attribute = Mapper.Map<AttributeValueDataModel, AttributeValue>(attribute);
            db.AttributeValue.Add(_attribute);

            //  }
            if (attr.Attribute.Count > 0)
            {
                foreach (var item in attr.Attribute)
                {
                    addFanbazarAttributes(item, itemid);
                }
            }
        }

        private int addFanbazarItem(ItemPostDataModel item, string username, bool flag = false)
        {
            string userid = Tools.UserID(username);
            var menu = db.Menu.FirstOrDefault(u => u.ID == item.F_MenuID && u.isDeleted == false && u.Status == true && u.F_UserID == userid);
            if (menu == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            Item _item = new Item();

            _item.CreatedDateOnUTC = DateTime.Now;
            _item.SubmitedState = "Sent";
            _item.F_UserID = flag ? userid : Tools.UserID();
            _item.F_MenuID = item.F_MenuID;
            _item.NumberOfVisitors = 0;
            _item.Image = item.Image;
            _item.Type = menu.Type;
            _item.Name = item.Name;
            _item.Description = item.Description;
            db.Item.Add(_item);
            db.SaveChanges();
            //foreach (var itemgallery in item.ItemGallery)
            //{
            //    addItemGallery(itemgallery, _item.ID);
            //}
            return _item.ID;
        }
        private int EditFanbazarItem(ItemPostDataModel item, string username)
        {
            string userid = Tools.UserID(username);

            var _item = db.Item.FirstOrDefault(u => u.ID == item.ID && u.F_MenuID == item.F_MenuID && u.Menu.isDeleted == false && u.Menu.Status == true);

            if (_item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }



            if (!string.IsNullOrEmpty(item.Image))
            {
                _item.Image = item.Image;
            }
            _item.Name = item.Name;
            _item.Description = item.Description;
            db.Entry(_item).State = EntityState.Modified;
            db.SaveChanges();
            //foreach (var itemgallery in item.ItemGallery)
            //{
            //    addItemGallery(itemgallery, _item.ID);
            //}
            return _item.ID;
        }

        private void addItemGallery(ItemGalleyDataModel itemgallery, int itemid)
        {
            if (itemgallery.ID == 0)
            {
                ItemGallery _itemgalley = Mapper.Map<ItemGalleyDataModel, ItemGallery>(itemgallery);
                _itemgalley.F_ItemID = itemid;
                db.ItemGallery.Add(_itemgalley);
            }
            else
            {
                var itmgallery = db.ItemGallery.Find(itemgallery.ID);
                Mapper.Map(itemgallery, itmgallery);
                db.Entry(itmgallery).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        private void FillAttributeGroup(ref AttributeGroupDataModel item, IQueryable<AttributeValue> attributevalue)
        {
            List<AttributeDataModel> newattributes = new List<AttributeDataModel>();
            foreach (var itm in item.Attribute)
            {
                AttributeDataModel atr = new AttributeDataModel();
                atr = itm;
                FillAttribute(ref atr, attributevalue);
                newattributes.Add(atr);
            }
            item.Attribute = newattributes;
            List<AttributeGroupDataModel> _groups = new List<AttributeGroupDataModel>();
            if (item.AttributeGroup1.Count > 0)
            {

                foreach (var itm in item.AttributeGroup1)
                {
                    AttributeGroupDataModel group = new AttributeGroupDataModel();
                    group = itm;
                    FillAttributeGroup(ref group, attributevalue);
                    _groups.Add(group);
                }

            }
            item.AttributeGroup1 = _groups;
        }

        private void FillAttribute(ref AttributeDataModel attribute, IQueryable<AttributeValue> attributevalue)
        {
            foreach (var item in attributevalue)
            {
                if (attribute.ID == item.F_AttributeID)
                    attribute.Value = item.Value;
            }
            List<AttributeItemDataModel> newlist = new List<AttributeItemDataModel>();
            if (attribute.AttributeItem1.Count > 0)
            {
                foreach (var _itm in attribute.AttributeItem1)
                {
                    foreach (var itm in attributevalue)
                    {
                        if (itm.F_AttributeItemID == _itm.ID)
                        {
                            AttributeItemDataModel atr = new AttributeItemDataModel();
                            atr = _itm;
                            FillAttributeItem(ref atr, attributevalue);
                            newlist.Add(atr);
                        }
                    }

                }

            }
            attribute.AttributeItem1 = newlist;
        }

        private void FillAttributeItem(ref AttributeItemDataModel attributeitem, IQueryable<AttributeValue> attributevalue)
        {
            /*  foreach (var item in attributevalue)
              {
                  if (attributeitem.ID == item.F_AttributeItemID)
                      attributeitem.Value = item.Value;
              }*/
            List<AttributeDataModel> newlist = new List<AttributeDataModel>();
            foreach (var item in attributeitem.Attribute)
            {
                AttributeDataModel atr = new AttributeDataModel();
                atr = item;
                FillAttribute(ref atr, attributevalue);
                newlist.Add(atr);
            }
            attributeitem.Attribute = newlist;
        }

        private bool AExists(int id)
        {
            /* var role = Tools.UserRole();
             if (role == "admin" || role == "Company")
                 return true;*/
            string usereid = Tools.UserID();
            return db.Item.Count(e => e.ID == id && e.F_UserID == usereid) > 0;
        }

    }
}