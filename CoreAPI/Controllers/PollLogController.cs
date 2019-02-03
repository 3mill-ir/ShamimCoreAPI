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
using System;
using System.Web;

namespace CoreAPI.Controllers
{
  
    public class PollLogController : ApiController
    {
         private Entities db = DBConnect.getConnection();
       // private Entities db = new Entities();
        public PollLogController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<PollLog, PollLogDataModel>();
                cfg.CreateMap<PollLogDataModel, PollLog>();

            });

        }

        // POST: api/ PollLog/
        [ResponseType(typeof(PollLogDataModel))]
        public async Task<IHttpActionResult> PostAnswer(PollLogDataModel a)
        {

            PollLog _a = Mapper.Map<PollLogDataModel, PollLog>(a);
            
            string userid = Tools.UserID();
            _a.F_UserID = userid;
            _a.CreatedOnUTC = DateTime.Now;
            var answer = await db.PollAnswer.Where(u => u.ID == a.F_PollAnswerID).FirstOrDefaultAsync();
            if (answer != null)
            {
                answer.Score += 1;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.PollLog.Add(_a);
            await db.SaveChangesAsync();

            return Ok("جواب جدید با موفقیت ثبت شد");
        }
        [Route("api/PollLog/PostAnswerUser")]
        [ResponseType(typeof(PollLogDataModel))]
        public async Task<IHttpActionResult> PostAnswer(PollLogDataModel a,string username)
        {

            PollLog _a = Mapper.Map<PollLogDataModel, PollLog>(a);

            string userid = Tools.UserID(username);
            _a.F_UserID = userid;
            _a.CreatedOnUTC = DateTime.Now;
            var answer = await db.PollAnswer.Where(u => u.ID == a.F_PollAnswerID).FirstOrDefaultAsync();
            if (answer != null)
            {
                answer.Score += 1;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.PollLog.Add(_a);
            await db.SaveChangesAsync();

            return Ok("جواب جدید با موفقیت ثبت شد");
        }
        [Route("api/PollLog/PostPolling")]
        [ResponseType(typeof(PollLogDataModel))]
        public async Task<IHttpActionResult> PostPolling(string IP,int ansId,string device)
        {
            PollLogDataModel a = new PollLogDataModel();
            a.CreatedOnUTC = DateTime.Now;
            a.Device = device;
            a.IPAddress = IP;
            a.F_PollAnswerID = ansId;
            PollLog _a = Mapper.Map<PollLogDataModel, PollLog>(a);

            string userid = Tools.UserID();
            _a.F_UserID = userid;
            var answer = await db.PollAnswer.Where(u => u.ID == a.F_PollAnswerID).FirstOrDefaultAsync();
            if (answer != null)
            {
                answer.Score += 1;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.PollLog.Add(_a);
            await db.SaveChangesAsync();

            return Ok("نظر با موفقیت ثبت شد");
        }


        [Route("api/PollLog/PostPollingUser")]
        [ResponseType(typeof(PollLogDataModel))]
        public async Task<IHttpActionResult> PostPolling(string IP, int ansId, string device, string username)
        {
            PollLogDataModel a = new PollLogDataModel();
            a.CreatedOnUTC = DateTime.Now;
            a.Device = device;

            if (IP.Equals(0))
            {
                a.IPAddress = HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                a.IPAddress = IP;
            }
            a.F_PollAnswerID = ansId;
            PollLog _a = Mapper.Map<PollLogDataModel, PollLog>(a);

            string userid = Tools.UserID(username);
            _a.F_UserID = userid;
            var answer = await db.PollAnswer.Where(u => u.ID == a.F_PollAnswerID).FirstOrDefaultAsync();
            if (answer != null)
            {
                answer.Score += 1;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.PollLog.Add(_a);
            await db.SaveChangesAsync();

            return Ok("نظر با موفقیت ثبت شد");
        }



        /// <summary>
        /// شرکت در نظر سنجی
        /// </summary>
        /// <param name="username"></param>
        /// <param name="IP"></param>
        /// <param name="ansId"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/PollLog/AddPollingUser")]
        [ResponseType(typeof(PollLogDataModel))]
        public async Task<IHttpActionResult> AddPollingUser(string username,string IP, int ansId, string device)
        {
            var IsFound = db.PollAnswer.FirstOrDefault(u => u.ID == ansId);
            if (IsFound != null)
            {
                if (IP.Equals("0"))
                {
                    IP = HttpContext.Current.Request.UserHostAddress;
                }
                IsFound.Score = IsFound.Score + 1;
                await db.SaveChangesAsync();
                PollLogManagement PLM = new PollLogManagement();
                PLM.AddPollLog(ansId, device, IP,db);

                return Ok("نظر با موفقیت ثبت شد");
            }
            else

                return BadRequest("نظر با موفقیت ثبت شد");


        }


        // GET: api/PollAnswer/

        [Route("api/PollLog/GetPollLog")]
        [ResponseType(typeof(PollLogDataModel))]
        public async Task<IHttpActionResult> GetPollLog(int id)
        {

            var A = await db.PollLog.FirstOrDefaultAsync();
            if (A == null)
            {
                return Content(HttpStatusCode.NotFound, "جواب با آیدی مورد نظر پیدا نشد");
            }

            return Ok(Mapper.Map<PollLog, PollLogDataModel>(A));

        }

        // PUT: api/PollAnswer/
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPollLog(int id, PollLogDataModel a)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != a.ID)
            {
                return Content(HttpStatusCode.NotFound, "اطلاعات ورودی همخوانی ندارند");
            }
            if (!AExists(id))
            {
                return Content(HttpStatusCode.NotFound, "جواب با آیدی مورد نظر پیدا نشد");
            }

            var _a =await db.PollLog.FindAsync(id);
            Mapper.Map(a,_a);

            db.Entry(_a).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return Ok("جواب مورد نظر ا موفقیت آپدیت شد");

        }

        // DELETE: api/PollAnswer/
        [ResponseType(typeof(PollLog))]
        public async Task<IHttpActionResult> DeletePollAnswer(int id)
        {

            if (!AExists(id))
            {
                return Content(HttpStatusCode.NotFound, "جواب با آیدی مورد نظر پیدا نشد");
            }
            PollLog a1 = await db.PollLog.FindAsync(id);
            db.PollLog.Remove(a1);
            await db.SaveChangesAsync();



            return Ok("جواب مورد نظر با موفقیت حذف شد");
        }

        private bool AExists(int id)
        {

            return db.PollAnswer.Count(e => e.ID == id) > 0;
        }

    }
}