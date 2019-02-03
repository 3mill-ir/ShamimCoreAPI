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
    public class PollAnswerController : ApiController
    {


         private Entities db = DBConnect.getConnection();
       // private Entities db = new Entities();
        public PollAnswerController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<PollLog, PollLogDataModel>();
                cfg.CreateMap<PollLogDataModel, PollLog>();

                cfg.CreateMap<PollAnswer, PollAnswerDataModel>();
                cfg.CreateMap<PollAnswerDataModel, PollAnswer>();

                cfg.CreateMap<PollAnswer, PollAnswerTextDataModel>();
                cfg.CreateMap<PollAnswerTextDataModel, PollAnswer>();

                cfg.CreateMap<PollAnswer, PollAnswerTextDataModel>().ReverseMap();
            });
        }
        // POST: api/PollAnswer/
        [Route("api/PollAnswer/PostAnswer")]
        [ResponseType(typeof(PollAnswerDataModel))]
        public async Task<IHttpActionResult> PostAnswer(PollAnswerDataModel a)
        {

            //  PollAnswer Answer = Mapper.Map<PollAnswerDataModel, PollAnswer>(a);
            PollAnswer Answer = new PollAnswer();
            Answer = await db.PollAnswer.FindAsync(a.ID);
           
            PollLog polllog = new PollLog();
            polllog.IPAddress = a.IPAddress;
            polllog.Device = a.Device;
            polllog.F_PollAnswerID = a.ID;
            polllog.CreatedOnUTC = System.DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (Answer.Score==null)
            {
                Answer.Score = 0;
            }

            Answer.Score = Answer.Score + 1;
            db.Entry(Answer).State = EntityState.Modified;

            db.PollLog.Add(polllog);
            await db.SaveChangesAsync();

            return Ok("پاسخ شما ثبت شد");
        }
        // POST: api/PollAnswer/
        [Route("api/PollAnswer/PostAnswerText")]
        [ResponseType(typeof(PollAnswerTextDataModel))]
        public async Task<IHttpActionResult> PostAnswerText(PollAnswerTextDataModel a)
        {

            PollAnswer Answer = Mapper.Map<PollAnswerTextDataModel, PollAnswer>(a);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }           
            db.PollAnswer.Add(Answer);         
            await db.SaveChangesAsync();
            return Ok("جواب جدید با موفقیت ثبت شد");
        }


        // GET: api/PollAnswer/

        [Route("api/PollAnswer/GetPollAnswer")]
        [ResponseType(typeof(PollAnswerDataModel))]
        public async Task<IHttpActionResult> GetPollAnswer(int id)
        {
            List<PollAnswerDataModel> _PollAnswerlist = new List<PollAnswerDataModel>();
            var Answer=  await db.PollAnswer.Where(u => u.F_PollQuestionID == id ).ToListAsync();


            
            if (Answer == null)
            {
                return Content(HttpStatusCode.NotFound, "جواب برای سوال مورد نظر پیدا نشد");
            }


            foreach (var m in Answer)
            {
                _PollAnswerlist.Add(Mapper.Map<PollAnswer, PollAnswerDataModel>(m));
            }

            return Ok(_PollAnswerlist);
             

        }

        // PUT: api/PollAnswer/
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPollAnswer(int id, PollAnswerDataModel a)
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

            var _a = await db.PollAnswer.FindAsync(id);
            Mapper.Map(a,_a);
          
            db.Entry(_a).State = EntityState.Modified;
            
            await db.SaveChangesAsync();
            return Ok("جواب مورد نظر ا موفقیت آپدیت شد");

        }

        // DELETE: api/PollAnswer/
        [ResponseType(typeof(PollAnswer))]
        public async Task<IHttpActionResult> DeletePollAnswer(int id)
        {

            // var areaName = values["area"];

            //   var routes = RouteTable.Routes.GetRouteData;
            //   var values = routes.Values;

            if (!AExists(id))
            {
                return Content(HttpStatusCode.NotFound, "جواب با آیدی مورد نظر پیدا نشد");
            }
            PollAnswer a1 = await db.PollAnswer.FindAsync(id);
            var pollings = db.PollLog.Where(u => u.F_PollAnswerID == id).ToList();
            foreach (var item in pollings)
            {
                db.PollLog.Remove(item);
            }
            db.PollAnswer.Remove(a1);
            await db.SaveChangesAsync();



            return Ok("جواب مورد نظر با موفقیت حذف شد");
        }

        private bool AExists(int id)
        {

            return db.PollAnswer.Count(e => e.ID == id ) > 0;
        }
    }
} 
