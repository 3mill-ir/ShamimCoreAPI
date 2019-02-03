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

namespace CoreAPI.Controllers
{
    public class PollQuestionController : ApiController
    {
          private Entities db = DBConnect.getConnection();
        //private Entities db = new Entities();
        public PollQuestionController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<PollQuestion, PollQuestionDataModel>();
                cfg.CreateMap<PollQuestionDataModel, PollQuestion>();

                cfg.CreateMap<PollAnswer, PollAnswerDataModel>();
                cfg.CreateMap<PollAnswerDataModel, PollAnswer>();
                cfg.CreateMap<PollLog, PollLogDataModel>();
                cfg.CreateMap<PollLogDataModel, PollLog>();

                 
            });

        }
    /// <summary>
    /// افزودن نظرسنجی جدید
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
        [ResponseType(typeof(PollQuestionDataModel))]
        public async Task<IHttpActionResult> PostQuestion(PollQuestionDataModel q)
        {
            string F_UserName= Tools.UserID(); 
  
            PollQuestionManagement poll = new PollQuestionManagement();

            int scale1 = poll.AddPollQuestion(q, F_UserName,db);
            if (scale1 == 2)
            {
                return BadRequest("Conflict");
            }

            await db.SaveChangesAsync();
        
            return Ok("سوال جدید با موفقیت ثبت شد");
        }


        /// <summary>
        /// لیست نظر سنجی ها
        /// </summary>
        /// <returns></returns>
        [Route("api/PollQuestion/GetPollQuestions")]
        [ResponseType(typeof(List<PollQuestionDataModel>))]
        public async Task<IHttpActionResult> GetPollQuestions()
        {
            List<PollQuestionDataModel> _list = new List<PollQuestionDataModel>();
            string userId = Tools.UserID();
            var QList = await db.PollQuestion.AsNoTracking().Where(u => u.F_UserID == userId && u.isDeleted == false).OrderBy(u=>u.ID).ToListAsync();

            foreach (var m in QList)
            {
                _list.Add(Mapper.Map<PollQuestion, PollQuestionDataModel>(m));
            }
            return Ok(_list);
        }

      /// <summary>
      /// جزییات نظر سنجی خاص
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
        [Route("api/PollQuestion/GetPollQuestion")]
        [ResponseType(typeof(PollQuestionDataModel))]
        public async Task<IHttpActionResult> GetPollQuestion(int id)
        {
            // db.Configuration.LazyLoadingEnabled = true;
            db.Configuration.LazyLoadingEnabled = true;   
            string userId = Tools.UserID();
            PollQuestion _nazar = new PollQuestion();
            var Q = await db.PollQuestion.Include(t=>t.PollAnswer).FirstOrDefaultAsync(u => u.F_UserID == userId && u.isDeleted == false && u.ID == id);                     
            if (Q == null)
            {
                return Content(HttpStatusCode.NotFound, "سوال با آیدی مورد نظر پیدا نشد");
            }
            return Ok(Mapper.Map<PollQuestion, PollQuestionDataModel>(Q));
        }
        //api:Get
        // in api baray bargardandan akharin nazarsanji faal estefade mishavad
        [Route("api/PollQuestion/GetPollActive")]
        [ResponseType(typeof(PollQuestionDataModel))]
        public async Task<IHttpActionResult> GetPollActive()
        {

            //  db.Configuration.LazyLoadingEnabled = true;
            Entities db = DBConnect.getEnabledLazyConnection();
            string userId = Tools.UserID();
            PollQuestion _nazar = new PollQuestion();
            var Q = await db.PollQuestion.AsNoTracking().Include(t => t.PollAnswer).FirstOrDefaultAsync(u => u.F_UserID == userId && u.isDeleted == false && u.EndDateOnUTC>=DateTime.UtcNow );
            if (Q == null)
            {
                return Content(HttpStatusCode.NotFound, "نظر سنجی فعالی وجود ندارد");
            }
            return Ok(Mapper.Map<PollQuestion, PollQuestionDataModel>(Q));
        }
   

        /// <summary>
        /// جزییات نظر سنجی فعال
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/PollQuestion/GetPollActiveUser")]
        [ResponseType(typeof(PollQuestionDataModel))]
        public async Task<IHttpActionResult> GetPollActive(string username)
        {

            // db.Configuration.LazyLoadingEnabled = true;
            Entities db = DBConnect.getEnabledLazyConnection();
            string userId = Tools.UserID(username);
            PollQuestion _nazar = new PollQuestion();
            var Q = await db.PollQuestion.AsNoTracking().Include(t => t.PollAnswer).FirstOrDefaultAsync(u => u.F_UserID == userId && u.isDeleted == false && u.EndDateOnUTC >= DateTime.UtcNow);
            if (Q == null)
            {
                return Content(HttpStatusCode.NotFound, "نظر سنجی فعالی وجود ندارد");
            }
            return Ok(Mapper.Map<PollQuestion, PollQuestionDataModel>(Q));
        }

     /// <summary>
     /// ویرایش یک نظرسنجی
     /// </summary>
     /// <param name="id"></param>
     /// <param name="q"></param>
     /// <returns></returns>
        [Route("api/PollQuestion/PutQuestion")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuestion(int id, PollQuestionDataModel q)
        {
            string F_UserName = Tools.UserID(); 

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != q.ID)
            {
                return Content(HttpStatusCode.NotFound, "اطلاعات ورودی همخوانی ندارند");
            }
            if (!QExists(id))
            {
                return Content(HttpStatusCode.NotFound, "سوال با آیدی مورد نظر پیدا نشد");
            }
            //string userid = Tools.UserID();
            //var uid = db.PollQuestion.Find(id).F_UserID;
            //if(uid!=userid)
            //{
            //    return Content(HttpStatusCode.Forbidden, "دسترسی به رسورس برای شما غیرمجاز است");//update because of security issues
            //}

            //var _q =await db.PollQuestion.FindAsync(id);
            //Mapper.Map(q,_q);
            //_q.F_UserID = userid;
            //db.Entry(_q).State = EntityState.Modified;
            //db.Entry(_q).Property(x => x.F_UserID).IsModified = false;


            PollQuestionManagement poll = new PollQuestionManagement();
            string result = poll.EditPollQuestion(q,F_UserName,db);
            if (result == "OK")
            {
                return Ok("سوال مورد نظر ا موفقیت آپدیت شد");
            }
            else
            {
                   return BadRequest(result);
            }

        }

      /// <summary>
      /// حذف سوالات مربوط به یک نظرسنجی
      /// </summary>
      /// <param name="PollAnswerId"></param>
      /// <param name="PollQuestionID"></param>
      /// <returns></returns>
        [Route("api/PollQuestion/DeletePollAnswer")]
        [ResponseType(typeof(PollQuestion))]
        public async Task<IHttpActionResult> DeletePollAnswer(int PollAnswerId, int PollQuestionID)
        {
            string F_UserName = Tools.UserID();
            PollAnswerManagement post = new PollAnswerManagement();
            string result = post.DeletePollAnswer(PollAnswerId, PollQuestionID, F_UserName, db);
            if (result == "NotFound")
            {
                return Content(HttpStatusCode.NotFound, "سوال با آیدی مورد نظر پیدا نشد");
            }
            else if (result == "MinCount")
            {
                return Content(HttpStatusCode.NotFound, "حداقل بایستی دو گزینه برای نظر سنجی وجود داشته باشد.");
            }
            await db.SaveChangesAsync();

            return Ok("سوال مورد نظر با موفقیت حذف شد");
        }

        /// <summary>
        /// حذف یک نظرسنجی
        /// </summary>
        /// <param name="PollQuestionID"></param>
        /// <returns></returns>
        [Route("api/PollQuestion/DeletePoll")]
        [ResponseType(typeof(PollQuestion))]
        public async Task<IHttpActionResult> DeletePoll(int PollQuestionID)
        {
            string F_UserName = Tools.UserID();

            var quest =await db.PollQuestion.Where(u => u.ID == PollQuestionID && u.F_UserID == F_UserName).FirstOrDefaultAsync();
            if (quest == null)
            {
                return Content(HttpStatusCode.NotFound, "سوال با آیدی مورد نظر پیدا نشد");
            }
            quest.isDeleted = true;

            await db.SaveChangesAsync();

            return Ok("سوال مورد نظر با موفقیت حذف شد");
        }


        private bool QExists(int id)
        {
           
            return db.PollQuestion.Count(e => e.ID == id && e.isDeleted == false) > 0;
        }
    }
}