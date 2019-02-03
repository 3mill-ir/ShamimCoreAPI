using CoreAPI.Models;
using CoreAPI.Models.BLL;
using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CoreAPI.Controllers
{
    public class RollController : ApiController
    {
        private Entities db = DBConnect.getConnection();

        [Route("api/Roll/GetListRollGroupe")]
        public async Task<IHttpActionResult> GetListRollGroupe()
        {
            string userId = Tools.UserID();
            var temp = await db.UserRollMapping.Include(u => u.RollGroupe).Where(u => u.F_UserID == userId && u.F_RollGroupeID != null).Select(u => u.RollGroupe).ToListAsync();
            return Ok(temp);
        }

        [Route("api/Roll/PostRollGroupe")]
        public async Task<IHttpActionResult> PostRollGroupe(RollGroupe model)
        {
            string userId = Tools.UserID();
            if (!db.RollGroupe.Any(u => u.GroupeName == model.GroupeName))
            {
                model.ID = IDGenerator(20);
                db.RollGroupe.Add(model);
                db.SaveChanges();
                if (userId == "00d8220d-dd4f-4be8-a352-de1e764b795d")
                {
                    UserRollMapping um = new UserRollMapping();
                    um.F_UserID = userId; um.F_RollGroupeID = model.ID;
                    db.UserRollMapping.Add(um);
                    db.SaveChanges();
                }
            }
            return Ok();
        }

        [Route("api/Roll/DeleteRollGroupe")]
        public async Task<IHttpActionResult> DeleteRollGroupe(string ID)
        {
            string userId = Tools.UserID();
            var temp = await db.RollGroupe.FirstOrDefaultAsync(u => u.ID == ID);
            if (temp == null)
                return NotFound();
            else
            {
                foreach (var Mapping in db.Roll_Mapping_RollGroupe.Where(u => u.F_RollGroupeID == ID))
                {
                    db.Roll_Mapping_RollGroupe.Remove(Mapping);
                }
                foreach (var UserRoll in db.UserRollMapping.Where(u => u.F_RollGroupeID == ID))
                {
                    db.UserRollMapping.Remove(UserRoll);
                }
                db.RollGroupe.Remove(temp);
                db.SaveChanges();
                return Ok();
            }
        }


        [Route("api/Roll/PostAsignRollToRollGroupe")]
        public async Task<IHttpActionResult> PostAsignRollToRollGroupe(List<AspNetRoles> model, string RollGroupeID)
        {
            string userId = Tools.UserID();
            var temp = await db.RollGroupe.FirstOrDefaultAsync(u => u.ID == RollGroupeID);
            if (temp == null)
                return NotFound();
            else
            {
                if (temp.Roll_Mapping_RollGroupe != null)
                    db.Roll_Mapping_RollGroupe.RemoveRange(temp.Roll_Mapping_RollGroupe);
                db.SaveChanges();
                foreach (var item in model)
                {
                    Roll_Mapping_RollGroupe m = new Roll_Mapping_RollGroupe();
                    m.F_RollGroupeID = RollGroupeID;
                    m.F_RollID = item.Id;
                    db.Roll_Mapping_RollGroupe.Add(m);
                }
                db.SaveChanges();
                return Ok();
            }
        }

        [Route("api/Roll/GetListRoll")]
        public async Task<IHttpActionResult> GetListRoll()
        {
            string userId = Tools.UserID();
            var temp = await db.UserRollMapping.Include(u => u.AspNetRoles).Where(u => u.F_UserID == userId && u.F_RollID != null).Select(u => u.AspNetRoles).ToListAsync();
            return Ok(temp);
        }

        [Route("api/Roll/GetListRollOfRollGroup")]
        public async Task<IHttpActionResult> GetListRollOfRollGroup(string RollGroupID)
        {
            var temp = db.Roll_Mapping_RollGroupe.Include(u => u.AspNetRoles).Where(u => u.F_RollGroupeID == RollGroupID).Select(q => q.AspNetRoles);
            return Ok(temp);
        }

        [Route("api/Roll/PostRoll")]
        public async Task<IHttpActionResult> PostRoll(AspNetRoles model)
        {
            var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            string userId = Tools.UserID();
            if (!roleManager.RoleExists(model.Name))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = model.Name;
                roleManager.Create(role);
                if (userId == "00d8220d-dd4f-4be8-a352-de1e764b795d")
                {
                    UserRollMapping um = new UserRollMapping();
                    um.F_UserID = userId; um.F_RollID = role.Id;
                    db.UserRollMapping.Add(um);
                    db.SaveChanges();
                }
                return Ok();
            }
            return InternalServerError();
        }


        [Route("api/Roll/DeleteRoll")]
        public async Task<IHttpActionResult> DeleteRoll(string ID)
        {
            string userId = Tools.UserID();
            var temp = await db.AspNetRoles.FirstOrDefaultAsync(u => u.Id == ID);
            if (temp == null)
                return NotFound();
            else
            {
                foreach (var Mapping in db.Roll_Mapping_RollGroupe.Where(u => u.F_RollID == ID))
                {
                    db.Roll_Mapping_RollGroupe.Remove(Mapping);
                }
                foreach (var UserRoll in db.UserRollMapping.Where(u => u.F_RollID == ID))
                {
                    db.UserRollMapping.Remove(UserRoll);
                }
                db.AspNetRoles.Remove(temp);
                db.SaveChanges();
                return Ok();
            }
        }

        [Route("api/Roll/PostAsignRollToUser")]
        public async Task<IHttpActionResult> PostAsignRollToUser(List<AspNetRoles> model, string F_UserID)
        {
            var mainQuery =await db.UserRollMapping.Include(u => u.AspNetRoles).Where(u => u.F_UserID == F_UserID && u.F_RollID != null).ToListAsync();
            var IDs2 = mainQuery.Select(q => q.AspNetRoles.Id).ToList();
            var RemoveObj = mainQuery.Except(mainQuery.Where(u => model.Select(w => w.Id).Contains(u.F_RollID))).ToList();
            var InsertObject = model.Except(model.Where(w => IDs2.Contains(w.Id)));
            if (RemoveObj != null)
                db.UserRollMapping.RemoveRange(RemoveObj);
            db.SaveChanges();
            foreach (var item in InsertObject)
            {
                UserRollMapping um = new UserRollMapping();
                um.F_RollID = item.Id; um.F_UserID = F_UserID;
                db.UserRollMapping.Add(um);
            }
            db.SaveChanges();
            return Ok();
        }

        [Route("api/Roll/PostAsignRollGroupToUser")]
        public async Task<IHttpActionResult> PostAsignRollGroupToUser(List<RollGroupe> model, string F_UserID)
        {
            var mainQuery =await db.UserRollMapping.Include(u => u.RollGroupe).Where(u => u.F_UserID == F_UserID && u.F_RollGroupeID != null).ToListAsync();
            var IDs2 = mainQuery.Select(q => q.RollGroupe.ID).ToList();
            var RemoveObj = mainQuery.Except(mainQuery.Where(u => model.Select(w => w.ID).Contains(u.F_RollGroupeID))).ToList();
            var InsertObject = model.Except(model.Where(w => IDs2.Contains(w.ID)));
            if (RemoveObj != null)
                db.UserRollMapping.RemoveRange(RemoveObj);
            db.SaveChanges();
            foreach (var item in InsertObject)
            {
                UserRollMapping um = new UserRollMapping();
                um.F_RollGroupeID = item.ID; um.F_UserID = F_UserID;
                db.UserRollMapping.Add(um);
            }
            db.SaveChanges();
            return Ok();
        }

        [Route("api/Roll/GetRollAuthorizedForUser")]
        public async Task<IHttpActionResult> GetRollAuthorizedForUser(string RollName)
        {
            string UserID = Tools.UserID();
            var temp = await db.UserRollMapping.Include(u => u.AspNetRoles).Include(q => q.RollGroupe).Where(u => u.F_UserID == UserID).ToListAsync();
            var Result = temp.Select(q => q.AspNetRoles).ToList();
            Result.AddRange(db.Roll_Mapping_RollGroupe.Include(w => w.AspNetRoles).Where(a => temp.Select(q => q.AspNetRoles.Id).Contains(a.AspNetRoles.Id)).Select(q => q.AspNetRoles));
            return Ok(Result.FirstOrDefault(q => q.Name == RollName) != null ? true : false);
        }

        [Route("api/Roll/GetAllRollsForUser")]
        public async Task<IHttpActionResult> GetAllRollsForUser()
        {
            string UserID = Tools.UserID();
            var temp = await db.UserRollMapping.Include(u => u.AspNetRoles).Include(q => q.RollGroupe).Where(u => u.F_UserID == UserID).ToListAsync();
            var Result = temp.Where(q=>q.F_RollID!=null).Select(q => q.AspNetRoles).ToList();
            var IDs = Result.Select(q => q.Id).ToList();
            var Result2 =await db.Roll_Mapping_RollGroupe.Include(w => w.AspNetRoles).Where(a => IDs.Contains(a.AspNetRoles.Id)).Select(q => q.AspNetRoles).ToListAsync();
            Result.AddRange(Result2);
            return Ok(Result);
        }

        [Route("api/Roll/GetListRollGroupOfUser")]
        public async Task<IHttpActionResult> GetListRollGroupOfUser(string UserID)
        {
            var temp = await db.UserRollMapping.Include(u => u.RollGroupe).Where(u => u.F_UserID == UserID && u.F_RollGroupeID != null).Select(u => u.RollGroupe).ToListAsync();
            return Ok(temp);
        }

        [Route("api/Roll/GetListRollOfUser")]
        public async Task<IHttpActionResult> GetListRollOfUser(string UserID)
        {
            var temp = await db.UserRollMapping.Include(u => u.AspNetRoles).Where(u => u.F_UserID == UserID && u.F_RollID != null).Select(u => u.AspNetRoles).ToListAsync();
            return Ok(temp);
        }





        public string IDGenerator(int length)
        {
            string result = "";
            bool loop = true;
            Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            do
            {
                string temp = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                var _object = db.Shekayat.FirstOrDefault(u => u.TrackingCode == temp);
                if (_object == null)
                {
                    result = temp;
                    loop = false;
                }
                else
                {
                    loop = true;
                }
            } while (loop);
            return result;
        }

    }
}
