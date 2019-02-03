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
using System.Web;

namespace CoreAPI.Controllers
{
    public class CommentController : ApiController
    {
         private Entities db = DBConnect.getConnection();
       // private Entities db = new Entities();
        public CommentController()
        {
            Mapper.Initialize(cfg=> 
            {
                cfg.CreateMap<Comments, CommentDataModel>();
                cfg.CreateMap<CommentDataModel, Comments>();
            });
        }
        /// <summary>
        /// افزودن کامنت جدید
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [Route("api/Comment/PostComment")]
        [ResponseType(typeof(CommentDataModel))]
        public async Task<IHttpActionResult> PostComment(CommentDataModel comment)
        {

            Comments _comment = Mapper.Map<CommentDataModel, Comments>(comment);
      
            Posts post= await db.Posts.FirstAsync(u => u.ID == comment.F_PostsID);
            post.NumberOfComments = post.NumberOfComments ?? default(int) + 1;
            if (string.IsNullOrEmpty(comment.IPAddress))
                _comment.IPAddress= HttpContext.Current.Request.UserHostAddress;

           _comment.F_UserID = post.F_UserID;
            _comment.CreatedDateOnUTC = DateTime.Now;


            db.Entry(post).State = EntityState.Modified;

            db.Comments.Add(_comment);
            await db.SaveChangesAsync();
            return Ok(post.NumberOfComments);

        }
     /// <summary>
     /// لیست همه کامنت ها
     /// </summary>
     /// <returns></returns>
        [Route("api/Comment/GetComments")]
        [ResponseType(typeof(List<CommentDataModel>))]
        public async Task<IHttpActionResult> GetComments()
        {
            List<CommentDataModel> _Commentlist = new List<CommentDataModel>();
            string userId = Tools.UserID();
            var CommentList = await db.Comments.Include(w=>w.Posts).Where(u => u.F_UserID == userId ).ToListAsync();

            foreach (var m in CommentList)
            {
                _Commentlist.Add(Mapper.Map<Comments, CommentDataModel>(m));
            }

            return Ok(_Commentlist);
        }
        //By ID
        //in api yek comment ra ba estefade az id barmigardanad
        [Route("api/Comment/GetComment")]
        [ResponseType(typeof(CommentDataModel))]
        public async Task<IHttpActionResult> GetComment(int id)
        {
            string userId = Tools.UserID();
            var comment = await db.Comments.Where(u => u.F_UserID == userId &&  u.ID == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(Mapper.Map<Comments, CommentDataModel>(comment));
        }

        //api:Put
        // in api baray update kardane har comment estefade mishavad
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int id, CommentDataModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.ID)
            {
                return Content(HttpStatusCode.BadRequest, "اطلاعات ورودی همخوانی ندارند");
            }
            if (!CommentExists(id))
            {
                return Content(HttpStatusCode.NotFound, "کامنت با آیدی مورد نظر پیدا نشد");
            }
            var _comment =await  db.Comments.FindAsync(id);
            Mapper.Map(comment,_comment);

            Posts post1 = await db.Posts.FirstAsync(u => u.ID == comment.F_PostsID);
            _comment.F_UserID = post1.F_UserID;
             
            db.Entry(_comment).State = EntityState.Modified;
           
            await db.SaveChangesAsync();
            return Ok("شما با موفقیت کامنت را آپدیت کردید");
        }
        /// <summary>
        /// افزودن دیس لایک به کامنت
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Comment/GetDislike")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetDislike(int id)
        {
            
            var comment = await db.Comments.Where(u =>u.ID == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                return Content(HttpStatusCode.NotFound, "کامنت با آیدی مورد نظر پیدا نشد");
            }
            if (comment.NumberOfDislikes == null)
            {
                comment.NumberOfDislikes = 0;
            }
            comment.NumberOfDislikes = comment.NumberOfDislikes + 1;
            await db.SaveChangesAsync();
            return Ok(comment.NumberOfDislikes);

        }

        /// <summary>
        /// تعداد دیسلایک های کامنت
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Comment/GetDislikes")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetDislikes(int id)
        {

            var comment = await db.Comments.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                return Content(HttpStatusCode.NotFound, "کامنت با آیدی مورد نظر پیدا نشد");
            }
            if (comment.NumberOfDislikes == null)
            {
                comment.NumberOfDislikes = 0;
            }       
            return Ok(comment.NumberOfDislikes);

        }

        /// <summary>
        /// افزودن لایک به کامنت
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Comment/GetLike")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetLike(int id)
        {

            var comment = await db.Comments.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (comment.NumberOfLikes == null)
            {
                comment.NumberOfLikes = 0;
            }
            comment.NumberOfLikes = comment.NumberOfLikes + 1;
            await db.SaveChangesAsync();
            return Ok(comment.NumberOfLikes);

        }

        /// <summary>
        /// تعداد لایک های کامنت
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Comment/GetLikes")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetLikes(int id)
        {

            var comment = await db.Comments.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            if (comment.NumberOfLikes == null)
            {
                comment.NumberOfLikes = 0;
            }
            return Ok(comment.NumberOfLikes);

        }
        //api:Delete
        //in api yek id daryaft karde va comment ra update mikonad
        [ResponseType(typeof(Comments))]
        public async Task<IHttpActionResult> DeleteComment(int id)
        {
            if (!CommentExists(id))
            {
                return Content(HttpStatusCode.NotFound, "کامنت با آیدی مورد نظر پیدا نشد");

            }
            Comments comment = await db.Comments.FindAsync(id);

            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
            
            return Ok("کامنت مرود نظر با موفقیت حذف شد");
        }


        /// <summary>
        /// تغییر وضعیت نمایش یک کامنت
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/Comment/DeleteChangeDisplayComment")]
        [ResponseType(typeof(Comments))]
        public async Task<IHttpActionResult> DeleteChangeDisplayComment(int id)
        {
            if (!CommentExists(id))
            {
                return Content(HttpStatusCode.NotFound, "کامنت مورد نظر پیدا نشد");

            }

            var comment = await db.Comments.FindAsync(id);
         
            comment.Dispaly = !comment.Dispaly;
            await db.SaveChangesAsync();
            return Ok("کامنت مورد نظر با موفقیت تغییر وضعیت شد");
        }

        private bool CommentExists(int id)
        {
            string userId = Tools.UserID();
            return db.Comments.Count(e => e.ID == id && e.F_UserID == userId ) > 0;
        }
    }
}