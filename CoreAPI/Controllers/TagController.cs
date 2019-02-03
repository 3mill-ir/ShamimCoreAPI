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
using CoreAPI.Models.BLL;
using AutoMapper;
using CoreAPI.Models;
using PagedList;

namespace CoreAPI.Controllers
{
    public class TagController : ApiController

    {
        private Entities db = DBConnect.getConnection();
        // private Entities db = new Entities();
        public TagController()
        {
            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<Tags, TagDataModel>().ForMember(dest => dest.F_UserID, opt => opt.MapFrom(src => src.F_UserID));
                cfg.CreateMap<TagDataModel, Tags>().ForMember(dest => dest.F_UserID, opt => opt.MapFrom(src => src.F_UserID));

                cfg.CreateMap<Tags, TagDataListModel>();
                cfg.CreateMap<TagDataListModel, Tags>();


                cfg.CreateMap<Tags_Posts_Mapping, TagsPostsMappingDataModel>();
                cfg.CreateMap<TagsPostsMappingDataModel, Tags_Posts_Mapping>();

                cfg.CreateMap<Posts, PostDataModel>();
                cfg.CreateMap<PostDataModel, Posts>();


            });
            //   db.Configuration.LazyLoadingEnabled = false;
        }
        //ezafe kardane tag va update table Tags_Posts_Mapping
        //post:api/Tag/
        [ResponseType(typeof(TagDataListModel))]
        public async Task<IHttpActionResult> AddTags(TagDataListModel tag, int id)//post id
        {
            Tags _tag = new Tags();
            TagsPostsMappingDataModel tagpost = new TagsPostsMappingDataModel();
            Tags_Posts_Mapping _tagpost;
            string UserID = Tools.RootUserID();
            var FoundedTags = db.Tags.Where(u => u.F_UserID == UserID && tag.ListTag.Select(e => e.Text).Contains(u.Text));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int i = -1;
            foreach (var item in tag.ListTag)
            {
                if (!FoundedTags.Select(u => u.Text).Contains(item.Text))
                {
                    _tag = Mapper.Map<TagDataModel, Tags>(item);
                    _tag.ID = i;
                    _tag.F_UserID = UserID;
                    db.Tags.Add(_tag);
                    i = _tag.ID;
                }
                else
                    i = item.ID;
                tagpost.F_PostsID = id;
                tagpost.F_TagsID = i;
                _tagpost = Mapper.Map<TagsPostsMappingDataModel, Tags_Posts_Mapping>(tagpost);
                db.Tags_Posts_Mapping.Add(_tagpost);
            }
            await db.SaveChangesAsync();
            return Ok();

        }


        //dar in class az gettag1 baray namayesh hame tag ha va az gettag2 baray namayesh bar hasbe filter id estefade shode ast
        // GET: api/Tags/


        [Route("api/Tag/GetTagPosts")]
        [ResponseType(typeof(List<TagDataModel>))]
        public async Task<IHttpActionResult> GetTagPosts(int id)//tag id

        {
            List<PostDataModel> _PostList = new List<PostDataModel>();
            string userId = Tools.UserID();
            var tagList = await db.Tags_Posts_Mapping.Include(p => p.Posts).Where(u => u.F_TagsID == id).ToListAsync();
            List<PostDataModel> _post = new List<PostDataModel>();
            //must be updated and join statement be replaced , this version is due to the emergency condition
            foreach (var item in tagList)
            {
                var post = db.Posts.Where(u => u.ID == item.F_PostsID && u.F_UserID == userId).FirstOrDefault();//conditions updated
                _post.Add(Mapper.Map<Posts, PostDataModel>(post));
            }

            return Ok(_post);
        }

        /// <summary>
        /// لیست پست هایی که آن تگ را دارند
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagenumber"></param>
        /// <returns></returns>
        [Route("api/Tag/GetTagPostsUser")]
        [ResponseType(typeof(List<TagDataModel>))]
        public async Task<IHttpActionResult> GetTagPosts(int id, string username, int pagesize, int pagenumber)//tag id

        {
            List<PostDataModel> _PostList = new List<PostDataModel>();
            string userId = Tools.UserID(username);
            var tagList = await db.Tags_Posts_Mapping.Where(u => u.F_TagsID == id).ToListAsync();
            List<PostDataModel> _post = new List<PostDataModel>();
            foreach (var item in tagList)
            {
                var post = db.Posts.Where(u => u.ID == item.F_PostsID && u.F_UserID == userId).FirstOrDefault();//conditions updated
                _post.Add(Mapper.Map<Posts, PostDataModel>(post));
            }


            return Ok(_post.ToPagedList(pagenumber, pagesize));
        }
        //dar in class az gettag1 baray namayesh hame tag ha va az gettag2 baray namayesh bar hasbe filter id estefade shode ast
        // GET: api/Tags/

        [Route("api/Tag/GetTag1")]
        [ResponseType(typeof(List<TagDataModel>))]
        public async Task<IHttpActionResult> GetTag()
        {
            List<TagDataModel> _Taglist = new List<TagDataModel>();
            string userId = Tools.UserID();
            var tagList = await db.Tags.Where(u => u.F_UserID == userId).ToListAsync();

            foreach (var m in tagList)
            {
                _Taglist.Add(Mapper.Map<Tags, TagDataModel>(m));
            }

            return Ok(_Taglist);
        }
        // PUT: api/Tags/
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTags(int id, TagDataModel tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.ID)
            {
                return Content(HttpStatusCode.NotFound, "اطلاعات ورودی همخوانی ندارند");
            }
            if (!tagExists(id))
            {
                return Content(HttpStatusCode.NotFound, "تگ با آیدی مورد نظر یافت نشد");

            }
            var _tag = await db.Tags.FindAsync(id);
            Mapper.Map(tag, _tag);
            db.Entry(_tag).State = EntityState.Modified;
            db.Entry(_tag).Property(x => x.F_UserID).IsModified = false;


            await db.SaveChangesAsync();

            return Ok("با موفقیت ویرایش شد");

        }

        // DELETE: api/Tags/
        [ResponseType(typeof(Tags))]
        public async Task<IHttpActionResult> DeleteTags(int id)
        {
            if (!tagExists(id))
            {
                return Content(HttpStatusCode.NotFound, "تگ با ایدی مورد نظر پیدا نشد");

            }
            Tags tag = await db.Tags.FindAsync(id);
            db.Tags.Remove(tag);
            await db.SaveChangesAsync();

            return Ok();
        }
        // GET: api/Post/id
        [Route("api/Tag/GetTag2")]
        [ResponseType(typeof(TagDataModel))]
        public async Task<IHttpActionResult> GetTag(int id)
        {
            string userId = Tools.UserID();
            var tag = await db.Tags.Where(u => u.F_UserID == userId && u.ID == id).FirstOrDefaultAsync();
            if (tag == null)
            {
                return NotFound();
            }


            return Ok(Mapper.Map<Tags, TagDataModel>(tag));

        }
        private bool tagExists(int id)
        {
            string userId = Tools.UserID();
            return db.Tags.Count(e => e.ID == id && e.F_UserID == userId) > 0;
        }

    }
}