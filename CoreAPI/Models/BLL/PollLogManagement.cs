using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.BLL
{
    public class PollLogManagement
    {
        /// <summary>
        /// In tabe jahate afzudane PollLog bar asase pasokhe morede nazar va ip ye device sherkat konande dar nazarsanji bekar gerefte mishavad
        /// </summary>
        /// <param name="F_PollAnswer_Id">type="int"</param>
        /// <param name="Device">type="string"</param>
        /// <returns>
        /// int (1)
        /// </returns>
        public int AddPollLog(int answerid,  string Device,string IP, Entities db)
        {

            PollLog InsertObject = new PollLog();
                //Jahate gereftan IP ye marbut be dastgahe sherkat konande dar nazarsanji
        
                //Marbut be IP haye local
                if (IP == "::1")
                    IP = "127.0.0.1";
                InsertObject.IPAddress = IP;
                InsertObject.CreatedOnUTC = DateTime.Now;
                InsertObject.Device = Device;
                InsertObject.F_PollAnswerID = answerid;
                db.PollLog.Add(InsertObject);
                db.SaveChanges();
                return 1;
            
        }



        //public int EditPollLog(PollLogModel model)
        //{
        //    using (BigMill.Models.ShahrdarEntities db = new ShahrdarEntities())
        //    {
        //        var EditObject = db.PollLogs.FirstOrDefault(u => u.ID == model.ID);
        //        if (EditObject != null)
        //        {
        //            EditObject.Address = model.Address;
        //            EditObject.CreatedOnUTC = DateTime.UtcNow;
        //            EditObject.Device = model.Device;
        //            EditObject.F_PollAnswer_Id = model.F_PollAnswer_Id;
        //            db.SaveChanges();
        //            return 1;
        //        }
        //        else
        //            return 2;
        //    }
        //}

        //public int DeletePollLog(PollLogModel model)
        //{
        //    using (BigMill.Models.ShahrdarEntities db = new ShahrdarEntities())
        //    {
        //        var DeleteObject = db.PollLogs.FirstOrDefault(u => u.ID == model.ID);
        //        if (DeleteObject != null)
        //        {
        //            db.PollLogs.Remove(DeleteObject);
        //            db.SaveChanges();
        //            return 1;
        //        }
        //        else
        //            return 2;
        //    }
        //}

        //public List<PollLogModel> ListPollLog()
        //{
        //    using (BigMill.Models.ShahrdarEntities db = new ShahrdarEntities())
        //    {
        //        var ListObject = db.PollLogs;
        //        List<PollLogModel> list = new List<PollLogModel>();
        //        foreach (var ListItem in ListObject)
        //        {
        //            PollLogModel t = new PollLogModel();
        //            t.Address = ListItem.Address;
        //            t.CreateOnUTC = ListItem.CreatedOnUTC??default(DateTime);
        //            t.Device = ListItem.Device;
        //            t.F_PollAnswer_Id = ListItem.F_PollAnswer_Id ?? default(int);
        //            t.ID = ListItem.ID;
        //            list.Add(t);
        //        }
        //        return list;
        //    }
        //}
    }
}