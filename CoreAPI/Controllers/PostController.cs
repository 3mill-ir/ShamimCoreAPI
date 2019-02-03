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
using PagedList;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Ajax.Utilities;
using CoreAPI.Models;
using System.Linq.Dynamic;

namespace CoreAPI.Controllers
{
    
    public class PostController : ApiController
    {
        private Entities db = DBConnect.getConnection();
        // private Entities db = new Entities();

        public PostController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Comments, CommentDataModel>();
                cfg.CreateMap<CommentDataModel, Comments>();

                cfg.CreateMap<Posts, PostDataModel>().ForMember(dest => dest.Comments1, opt => opt.MapFrom(src => src.Comments)).ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.Name));
                cfg.CreateMap<PostDataModel, Posts>().ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments1));

                cfg.CreateMap<Tags, TagDataModel>();
                cfg.CreateMap<TagDataModel, Tags>();

                cfg.CreateMap<Tags_Posts_Mapping, TagsPostsMappingDataModel>();
                cfg.CreateMap<TagsPostsMappingDataModel, Tags_Posts_Mapping>();
                cfg.CreateMap<NewsDataModel, Posts>();
                cfg.CreateMap<Posts, NewsDataModel>();
            });
            //  db.Configuration.LazyLoadingEnabled = false;
        }
       

        /// <summary>
        /// لیست جستجو در پست
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Route("api/Post/GetSearched")]
        [ResponseType(typeof(List<PostDataModel>))]
        public async Task<IHttpActionResult> GetSearched(string searchString)
        {
            List<PostDataModel> _Postlist = new List<PostDataModel>();
            //string userId = Tools.UserID();
            string userId = Tools.RootUserID();
            var PostList = await db.Posts.Where(u => (u.isDeleted == false) && (u.Tittle.Contains(searchString) || u.Description.Contains(searchString) || u.Detail.Contains(searchString))).ToListAsync();

            foreach (var m in PostList)
            {
                _Postlist.Add(Mapper.Map<Posts, PostDataModel>(m));
            }

            return Ok(_Postlist.ToList().ToPagedList(1, 15));
        }
        [AllowAnonymous]
        [Route("api/Post/GetSearchedUser")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetSearched(string searchString, string username)
        {
            List<PostDataModel> _Postlist = new List<PostDataModel>();
            string userId = Tools.UserID(username);
            var PostList = await db.Posts.Where(u => (u.isDeleted == false && u.Status == true) && (u.Tittle.Contains(searchString) || u.Description.Contains(searchString) || u.Detail.Contains(searchString))).ToListAsync();

            foreach (var m in PostList)
            {
                _Postlist.Add(Mapper.Map<Posts, PostDataModel>(m));
            }

            return Ok(_Postlist.ToList().ToPagedList(1, 15));
        }
       /// <summary>
       /// افزودن پست جدید
       /// </summary>
       /// <param name="post"></param>
       /// <returns></returns>
        [Route("api/Post/AddPost")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> AddPost(PostDataModel post)
        {
            PersianCalendar pc = new PersianCalendar();
            Posts _post = Mapper.Map<PostDataModel, Posts>(post);
            //_post.F_UserID = Tools.UserID();
            _post.F_UserID = Tools.RootUserID();


            _post.CreatedOnUTC = DateTime.Now;


            _post.isDeleted = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            db.Posts.Add(_post);
            await db.SaveChangesAsync();

            return Ok(_post.ID);
        }

      /// <summary>
      /// ویرایش پست 
      /// </summary>
      /// <param name="id"></param>
      /// <param name="post"></param>
      /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPost(int id, PostDataModel post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != post.ID)
            {
                return Content(HttpStatusCode.NotFound, "اطلاعات ورودی باهم همخوانی ندارند");
            }
            if (!PostExists(id))
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            //string userid = Tools.UserID();
            string userid = Tools.RootUserID();
            var uid = db.Posts.Find(id).F_UserID;
            if (userid != uid)
            {
                return Content(HttpStatusCode.Forbidden, "تلاش برای دسترسی به رسورس غیرمجاز");
            }

            post.isDeleted = false;
            //  post.Status = true;
            var _post = await db.Posts.FindAsync(id);

            Mapper.Map(post, _post);
            _post.F_UserID = Tools.UserID();
            db.Entry(_post).State = EntityState.Modified;
            db.Entry(_post).Property(x => x.F_UserID).IsModified = false;
            if(!_post.CreatedOnUTC.HasValue)
            db.Entry(_post).Property(x => x.CreatedOnUTC).IsModified = false;

            await db.SaveChangesAsync();

            return Ok();

        }
        [Route("api/Post/PutPostTags")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPostTags(int id, TagDataListModel tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!PostExists(id))
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            string F_UserID = Tools.RootUserID();
            var tagpost = await db.Tags_Posts_Mapping.Where(u => u.F_PostsID == id).ToListAsync();
            var tags = await db.Tags.Where(u => u.F_UserID == F_UserID).ToListAsync();
            foreach (var item in tagpost)
            {
                db.Tags_Posts_Mapping.Remove(item);
            }

            TagsPostsMappingDataModel tagposts = new TagsPostsMappingDataModel();
            Tags_Posts_Mapping _tagpost;
            foreach (var item in tag.ListTag)
            {
                var temp = tags.FirstOrDefault(u => u.Text == item.Text);
                if(temp==null)
                {
                    var temptag = new Tags() {F_UserID=F_UserID,Text=item.Text };
                    db.Tags.Add(temptag);
                    tagposts.F_TagsID = temptag.ID;
                }
                else
                    tagposts.F_TagsID = temp.ID;
                tagposts.F_PostsID = id;
                _tagpost = Mapper.Map<TagsPostsMappingDataModel, Tags_Posts_Mapping>(tagposts);
                db.Tags_Posts_Mapping.Add(_tagpost);
            }
            await db.SaveChangesAsync();
            return Ok();
        }

   /// <summary>
   /// حذف یک پست
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
        [Route("api/Post/DeletePost")]
        [ResponseType(typeof(Posts))]
        public async Task<IHttpActionResult> DeletePost(int id)
        {
            if (!PostExists(id))
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            var post =await db.Posts.FindAsync(id);
            post.isDeleted = true;
            CascadeTools ct = new CascadeTools();
            while (post.Comments.Count>0)
            {
               ct.DeleteCascadeComments(post.Comments.FirstOrDefault(),db);
            }

            await db.SaveChangesAsync();

            return Ok("پست مورد نظر با موفقیت حذف شد");
        }

  
      /// <summary>
      /// جزییات یک پست برای پروفایل لوگین شده
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
        [Route("api/Post/GetPostDetails")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetPostDetails(int id)//post id
        {
           
            //string userId = Tools.UserID();
            string userId = Tools.RootUserID();
            var post = await db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            return Ok(Mapper.Map<Posts, PostDataModel>(post));
        }

        [AllowAnonymous]
        [Route("api/Post/GetPostDetail")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetPostDetail(int id, string username)//for android
        {
            string userid = Tools.UserID(username);
            var ldb = DBConnect.getEnabledLazyConnection();
            var post = await ldb.Posts.Where(u => u.isDeleted == false && u.ID == id && u.F_UserID == userid).FirstOrDefaultAsync();

            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            List<Comments> comments = post.Comments.Where(u => u.Dispaly == true).ToList<Comments>();
            post.Comments = comments;
            NewsDataModel news = new NewsDataModel();
            news = Mapper.Map<Posts, NewsDataModel>(post);
            news.Like = "http://parkapi.3mill.ir/api/Post/GetLike?id=" + post.ID;
            news.Dislike = "http://parkapi.3mill.ir/api/Post/GetDisLike?id=" + post.ID;
            news.addComment = "http://parkapi.3mill.ir/Comment/PostComment";
            news.ImagePath= Tools.ReturnPath("DynamicPageImages", Tools.UserName(username)) + news.ImagePath;
            NewsList newslist = new NewsList();
            List<PostDataModel> _Postlist = new List<PostDataModel>();
            var PostList = await db.Posts.Where(u => (u.isDeleted == false && u.Status == true) && (u.Tittle.Contains(post.Tittle) || u.Description.Contains(post.Tittle) || u.Detail.Contains(post.Tittle))).ToListAsync();

            foreach (var m in PostList)
            {
                if (m.ID != post.ID)
                {
                    _Postlist.Add(Mapper.Map<Posts, PostDataModel>(m));
                }
            }

            List<ItemModel> items = new List<ItemModel>();
            foreach (var p in _Postlist)
            {
                ItemModel itemnews = new ItemModel();
                itemnews.Content = p.Description;
                itemnews.ID = p.ID;
                itemnews.Date = p.CreatedOnUTC;
                itemnews.Image = Tools.ReturnPath("DynamicPageImages", Tools.UserName(username)) + p.ImagePath;
                itemnews.Url = "http://parkapi.3mill.ir/api/Post/GetPostDetail?id=" + p.ID + "&&username=" + username;
                itemnews.Functionality = "NewsDetails";
                items.Add(itemnews);
            }
            newslist.Item = items;
            news.RelatedTopics = newslist;
            return Ok(news);
        }
 
     /// <summary>
     /// جزییات پست خاص
     /// </summary>
     /// <param name="username"></param>
     /// <param name="lang"></param>
     /// <param name="id"></param>
     /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetPostDetailsUser")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetPostDetails(string username, string lang,int id)//post id
        {

            string userId = Tools.UserID(username);
            var post = await db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Status == true && u.ID == id && u.Language == lang).FirstOrDefaultAsync();

            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            return Ok(Mapper.Map<Posts, PostDataModel>(post));
        }


       /// <summary>
       /// تغییر وضعیت یک پست
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        [Route("api/Post/DeleteStstus")]
        [ResponseType(typeof(Posts))]
        public async Task<IHttpActionResult> DeleteStstus(int id)
        {
            if (!PostExists(id))
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            Posts post = await db.Posts.FindAsync(id);

            post.Status = true;

            await db.SaveChangesAsync();

            return Ok("پست مورد نظر با موفقیت تغییر حالت داده شد");
        }

       /// <summary>
       /// تعداد لایک های یک پست
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetLike")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetLike(int id)
        {

            var post = await db.Posts.Where(u => u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            if (post.NumberOfLikes == null)
            {
                post.NumberOfLikes = 0;
            }
            post.NumberOfLikes = post.NumberOfLikes + 1;
            await db.SaveChangesAsync();
            return Ok(post.NumberOfLikes);
        }
        [AllowAnonymous]
        [Route("api/Post/GetLikes")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetLikes(int id)
        {

            var post = await db.Posts.Where(u => u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            if (post.NumberOfLikes == null)
            {
                post.NumberOfLikes = 0;
            }
            await db.SaveChangesAsync();
            return Ok(post.NumberOfLikes);
        }
        /// <summary>
        /// لیست کامنت های یک پست خاص
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Post/GetPostComment")]
        [ResponseType(typeof(List<CommentDataModel>))]
        public async Task<IHttpActionResult> GetPostComment(int id)//post id
        {
            List<CommentDataModel> _listcomment = new List<CommentDataModel>();
            var comment = await db.Comments.Where(u => u.F_PostsID == id&&u.Dispaly==true).ToListAsync();
            if (comment == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            foreach (var item in comment)
            {
                _listcomment.Add(Mapper.Map<Comments, CommentDataModel>(item));
            }


            return Ok(_listcomment);
        }

/// <summary>
/// افزودن دیس لایک به پست
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetDislike")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetDislike(int id)
        {
            var post = await db.Posts.Where(u => u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            if (post.NumberOfDislikes == null)
            {
                post.NumberOfDislikes = 0;
            }
            post.NumberOfDislikes = post.NumberOfDislikes + 1;
            await db.SaveChangesAsync();
            return Ok(post.NumberOfDislikes);

        }


        /// <summary>
        /// تعداددیس لایک پست
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetDislikes")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetDislikes(int id)
        {
            var post = await db.Posts.Where(u => u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            if (post.NumberOfDislikes == null)
            {
                post.NumberOfDislikes = 0;
            }
            await db.SaveChangesAsync();
            return Ok(post.NumberOfDislikes);

        }

        [Route("api/Post/GetVisitor")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetVisitor(int id)
        {
            var post = await db.Posts.Where(u => u.isDeleted == false && u.ID == id).FirstOrDefaultAsync();
            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");
            }
            if (post.NumberOfVisitors == null)
            {
                post.NumberOfVisitors = 0;
            }
            post.NumberOfVisitors = post.NumberOfVisitors + 1;


            await db.SaveChangesAsync();
            return Ok(post.NumberOfVisitors);

        }

     /// <summary>
     /// لیست پست ها بر اساس فیلتر های اعمال شده برای پروفایل لوگین شده 
     /// </summary>
     /// <param name="postfilterdatamodel"></param>
     /// <returns></returns>
        [Route("api/Post/postListPosts")]
        [ResponseType(typeof(ListPostDataModel))]
        public async Task<IHttpActionResult> postListPosts(FilterPostDatamodel postfilterdatamodel)
        {
            ListPostDataModel _Postlist = new ListPostDataModel();
            //string userId = Tools.UserID();
            string userId = Tools.RootUserID();
            var LastPostList = db.Posts.Include(mbox => mbox.Menu).Where(u =>
              u.isDeleted == false && u.F_UserID == userId &&
              (postfilterdatamodel.MenuId == 0 | u.F_MenuID == postfilterdatamodel.MenuId) &&
              (string.IsNullOrEmpty(postfilterdatamodel.Language) | u.Language == postfilterdatamodel.Language) &&
              (postfilterdatamodel.FromTime == null | u.CreatedOnUTC >= postfilterdatamodel.FromTime) &&
              (postfilterdatamodel.ToTime == null | u.CreatedOnUTC <= postfilterdatamodel.ToTime) &&
             (string.IsNullOrEmpty(postfilterdatamodel.search) |
             (postfilterdatamodel.type == 1 && u.Tittle.Contains(postfilterdatamodel.search)) ||
             (postfilterdatamodel.type == 2 && (u.Tittle.Contains(postfilterdatamodel.search) || u.Description.Contains(postfilterdatamodel.search)))
             )
              );
            //if (postfilterdatamodel.MenuId == 0)
            //{
            //   LastPostList = await db.Posts.Where(u => u.isDeleted == false && u.F_UserID == userId && postfilterdatamodel.FromTime==null).ToListAsync();
            //}
            //else
            //{
            //  LastPostList = await db.Posts.Where(u => u.F_MenuID == postfilterdatamodel.MenuId && u.isDeleted == false && u.F_UserID == userId).ToListAsync();
            //}
            //if (!string.IsNullOrEmpty(postfilterdatamodel.Language))
            //{
            //    LastPostList = (from post in LastPostList where post.Language == postfilterdatamodel.Language select post).ToList();
            //}
            //if (postfilterdatamodel.FromTime != null && postfilterdatamodel.ToTime != null)
            //{
            //    LastPostList = (from post in LastPostList where post.CreatedOnUTC >= postfilterdatamodel.FromTime && post.CreatedOnUTC <= postfilterdatamodel.ToTime select post).ToList();
            //}
            //if (!string.IsNullOrEmpty(postfilterdatamodel.search))
            //{
            //    LastPostList = (from post in LastPostList where post.Tittle.Contains(postfilterdatamodel.search) || post.Description.Contains(postfilterdatamodel.search) || post.Detail.Contains(postfilterdatamodel.search) select post).ToList();
            //}
            string sort_param = "";
            switch (postfilterdatamodel.sortby)
            {
                case "DateNew":
                    sort_param = "CreatedOnUTC Descending";
                    break;
                case "DateOld":
                    sort_param = "CreatedOnUTC Ascending";
                    break;
                case "NumberOfLikes":
                    sort_param = "NumberOfLikes Descending";
                    break;
                case "NumberOfDislikes":
                    sort_param = "NumberOfDislikes Descending";
                    break;
                case "NumberOfVisitors":
                    sort_param = "NumberOfVisitors Descending";
                    break;
                default:
                    sort_param = "ID Descending";
                    break;
            }


            var tempLastPostList = await LastPostList.OrderBy(sort_param).Skip((postfilterdatamodel.PageNumber - 1) * postfilterdatamodel.PageSize).Take(postfilterdatamodel.PageSize).ToListAsync();

            foreach (var m in tempLastPostList)
            {
                _Postlist.List.Add(Mapper.Map<Posts, PostDataModel>(m));
            }
            _Postlist.count = LastPostList.Count();
            //if (postfilterdatamodel.PageNumber != 0 && postfilterdatamodel.PageSize != 0)
            //{
            //    return Ok(_Postlist.ToList().ToPagedList(postfilterdatamodel.PageNumber, postfilterdatamodel.PageSize));//_Postlist.ToPagedList(pagenumber,pagesize
            //}
            return Ok(_Postlist);//_Postlist.ToPagedList(pagenumber,pagesize)
        }
        /// <summary>
        /// لیست پست های برای اندروید
        /// </summary>
        /// <param name="username"></param>
        ///    /// <param name="menuId"></param>
        /// <returns></returns>
        [Route("api/Post/GetListPostsAndroid")]
        [ResponseType(typeof(ListPostDataModel))]
        public IHttpActionResult GetListPostsAndroid(string username,int menuId)
        {
            ListPostDataModel _Postlist = new ListPostDataModel();
            string userId = Tools.UserID(username);
            var LastPostList = db.Posts.Where(u => u.isDeleted == false && u.Status == true && u.F_UserID == userId && u.F_MenuID==menuId);

            foreach (var m in LastPostList)
            {
                _Postlist.List.Add(Mapper.Map<Posts, PostDataModel>(m));
            }

            return Ok(_Postlist);//_Postlist.ToPagedList(pagenumber,pagesize)
        }

        /// <summary>
        /// لیست پست ها
        /// </summary>
        /// <param name="postfilterdatamodel"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Post/postListPostsuser")]
        [ResponseType(typeof(ListPostDataModel))]
        public IHttpActionResult postListPostsuser(FilterPostDatamodel postfilterdatamodel,string username)
        {
            ListPostDataModel _Postlist = new ListPostDataModel();
            string userId = Tools.UserID(username);
            var LastPostList = db.Posts.Include(mbox => mbox.Menu).Where(u =>
              u.isDeleted == false && u.Status==true && u.F_UserID == userId &&
              (postfilterdatamodel.MenuId == 0 | u.F_MenuID == postfilterdatamodel.MenuId) &&
              (string.IsNullOrEmpty(postfilterdatamodel.Language) | u.Language == postfilterdatamodel.Language) &&
              (postfilterdatamodel.FromTime == null | u.CreatedOnUTC >= postfilterdatamodel.FromTime) &&
              (postfilterdatamodel.ToTime == null | u.CreatedOnUTC <= postfilterdatamodel.ToTime) &&
             (string.IsNullOrEmpty(postfilterdatamodel.search) |
             (postfilterdatamodel.type == 1 && u.Tittle.Contains(postfilterdatamodel.search)) ||
             (postfilterdatamodel.type == 2 && (u.Tittle.Contains(postfilterdatamodel.search) || u.Description.Contains(postfilterdatamodel.search)))
             )
              );
            string sort_param = "";
            switch (postfilterdatamodel.sortby)
            {
                case "DateNew":
                    sort_param = "CreatedOnUTC Descending";
                    break;
                case "DateOld":
                    sort_param = "CreatedOnUTC Ascending";
                    break;
                case "NumberOfLikes":
                    sort_param = "NumberOfLikes Descending";
                    break;
                case "NumberOfDislikes":
                    sort_param = "NumberOfDislikes Descending";
                    break;
                case "NumberOfVisitors":
                    sort_param = "NumberOfVisitors Descending";
                    break;
                default:
                    sort_param = "ID Descending";
                    break;
            }


            var tempLastPostList =  LastPostList.OrderBy(sort_param).Skip((postfilterdatamodel.PageNumber - 1) * postfilterdatamodel.PageSize).Take(postfilterdatamodel.PageSize).ToList();

            foreach (var m in tempLastPostList)
            {
                _Postlist.List.Add(Mapper.Map<Posts, PostDataModel>(m));
            }
            _Postlist.count = LastPostList.Count();
            //if (postfilterdatamodel.PageNumber != 0 && postfilterdatamodel.PageSize != 0)
            //{
            //    return Ok(_Postlist.ToList().ToPagedList(postfilterdatamodel.PageNumber, postfilterdatamodel.PageSize));//_Postlist.ToPagedList(pagenumber,pagesize
            //}
            return Ok(_Postlist);//_Postlist.ToPagedList(pagenumber,pagesize)
        }
        //All
        [Route("api/Post/GetPostTags")]
        [ResponseType(typeof(TagDataModel))]
        public async Task<IHttpActionResult> GetPostTags(int id)//post id
        {
            List<TagDataModel> _Taglist = new List<TagDataModel>();
            //string userId = Tools.UserID();
            string userId = Tools.RootUserID();
            var Taglist = await db.Tags_Posts_Mapping.Where(u => u.F_PostsID == id).ToListAsync();

            //must be updated for using join
            foreach (var item in Taglist)
            {
                var tag = db.Tags.Where(u => u.ID == item.F_TagsID).FirstOrDefault();
                _Taglist.Add(Mapper.Map<Tags, TagDataModel>(tag));
            }
            return Ok(_Taglist);
        }
       /// <summary>
       /// لیست تگ های یک پست
       /// </summary>
       /// <param name="id"></param>
       /// <param name="username"></param>
       /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetPostTagsUser")]
        [ResponseType(typeof(List<TagDataModel>))]
        public async Task<IHttpActionResult> GetPostTags(int id, string username)//post id
        {
            List<TagDataModel> _Taglist = new List<TagDataModel>();
            string userId = Tools.UserID(username);
            var Taglist = await db.Tags_Posts_Mapping.Where(u => u.F_PostsID == id).ToListAsync();

            //must be updated for using join
            foreach (var item in Taglist)
            {
                var tag = db.Tags.Where(u => u.ID == item.F_TagsID && u.F_UserID == userId).FirstOrDefault();
                _Taglist.Add(Mapper.Map<Tags, TagDataModel>(tag));
            }
            return Ok(_Taglist);
        }

        //GET
        //return kardane post haye yek menu,baraye user
        [AllowAnonymous]
        [Route("api/Post/GetMenuPostsUser")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetMenuPosts(int id, string username, int pagenumber, int pagesize)//menu id
        {

            List<PostDataModel> _list = new List<PostDataModel>();
            string userId = Tools.UserID(username);
            var post = await db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.F_MenuID == id && u.Status == true).ToListAsync();
            if (post == null)
            {
                return Content(HttpStatusCode.NotFound, "پست با آیدی مورد نظر پیدا نشد");

            }
            foreach (var m in post)
            {
                _list.Add(Mapper.Map<Posts, PostDataModel>(m));
            }

            return Ok((_list.ToPagedList(pagenumber, pagesize)));
        }


        /// <summary>
        /// لیست آخرین پست ها
        /// </summary>
        /// <param name="count"></param>
        /// <param name="username"></param>
        /// <param name="menuid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetRecentPostsUser")]
        [ResponseType(typeof(List<PostDataModel>))]
        public async Task<IHttpActionResult> GetRecentPostsUser(int count, string username, int menuid)//menu id
        {

            List<PostDataModel> _list = new List<PostDataModel>();

            string userId = Tools.UserID(username);

            List<Posts> _listpost = new List<Posts>();
            if (menuid == -1) {
                _listpost = await db.Posts.Where(u => u.F_UserID == userId &&  u.isDeleted == false && u.Status == true).OrderByDescending(x => x.CreatedOnUTC).Take(count).ToListAsync<Posts>();
            }else {
                _listpost = await db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Status == true && u.F_MenuID == menuid).OrderByDescending(x => x.CreatedOnUTC).Take(count).ToListAsync<Posts>();
            }
           // var posts = await db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Status == true && u.F_MenuID == menuid).OrderByDescending(x => x.CreatedOnUTC).Take(count).ToListAsync<Posts>();
            //  posts.OrderByDescending(x => x.CreatedOnUTC);


            foreach (var m in _listpost)
            {
                _list.Add(Mapper.Map<Posts, PostDataModel>(m));
            }


            return Ok(_list.Take(count));
        }
        [AllowAnonymous]
        [Route("api/Post/GetLatestPostsUser")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetLatestPosts(int count, string username)//menu id
        {

            List<PostDataModel> _list = new List<PostDataModel>();

            string userId = Tools.UserID(username);
            var posts = await db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Status == true).OrderByDescending(x => x.CreatedOnUTC).ToListAsync<Posts>();
            //  posts.OrderByDescending(x => x.CreatedOnUTC);


            foreach (var m in posts)
            {
                _list.Add(Mapper.Map<Posts, PostDataModel>(m));
            }


            return Ok(_list.Take(count));
        }


        /// <summary>
        /// لیست محبوب ترین پست ها
        /// </summary>
        /// <param name="username"></param>
        /// <param name="count"></param>
        /// <param name="menuid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/Post/GetPopularPostsUser")]
        [ResponseType(typeof(List<PostDataModel>))]
        public async Task<IHttpActionResult> GetPopularPostsUser(string username, int count, int menuid)
        {
            List<PostDataModel> _list = new List<PostDataModel>();
            string userid = Tools.UserID(username);
            var posts = await db.Posts.Where(u => u.F_UserID == userid && u.isDeleted == false && u.Status == true && u.F_MenuID == menuid).OrderByDescending(x => x.CreatedOnUTC).OrderByDescending(u => u.NumberOfLikes ).Take(count).ToListAsync<Posts>();
            foreach (var item in posts)
            {
                _list.Add(Mapper.Map<Posts, PostDataModel>(item));
            }
            return Ok(_list.Take(count));
        }


        private bool PostExists(int id)
        {
            //string userId = Tools.UserID();
            string userId = Tools.RootUserID();
            return db.Posts.Count(e => e.ID == id && e.F_UserID == userId && e.isDeleted == false) > 0;
        }
        private static DateTime ConvertToPersian(string dt)
        {
            char[] delimiterChars = { '/' };
            string[] data = dt.Split(delimiterChars);
            string year = data[0];
            string month = data[1];
            string day = data[2];


            PersianCalendar pc = new PersianCalendar();
            DateTime date = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), pc);
            return date;
        }

    }
}