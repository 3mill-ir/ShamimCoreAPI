using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoreAPI.Models.BLL
{
    public class PollAnswerManagement
    {
        /// <summary>
        /// In tabe jahate afzudane gozineye pasokh be Soale morede nazar mibashad
        /// </summary>
        /// <param name="model">type="PollQuestionModel"</param>
        /// <returns>
        /// int (Adade 1 ra bar migardanad)
        /// </returns>
        public int AddPollAnswer(PollQuestionDataModel model, Entities db)
        {
          
                int t = 1;
                foreach (var item in model.PollAnswer)
                {
                    PollAnswer pol = new PollAnswer();
                    pol.Text = item.Text;
                    pol.F_PollQuestionID = model.ID;
                    pol.Color = item.Color;
                    pol.AnswerKey = t++;
              
                    //Score harkodam ra besurate pishfarz 1 dar nazar migirim ta nemudare rasm shode baraye Nazarsanji khali nabashad.
                    pol.Score = 1;
                    db.PollAnswer.Add(pol);
                }
                db.SaveChanges();
                return 1;
            
        }


        /// <summary>
        /// in tabe zamani bekar borde mishavad ke hengame virayeshe gozine haye yek soal bekhahim gozine i jadid ezafe konim
        /// </summary>
        /// <param name="AnswerText">type="string"</param>
        /// <param name="F_PollQuestion_Id">type="int"</param>
        /// <param name="MaxKeyValue">type="int"</param>
        private void AddPollAnswerEditHelper(string AnswerText, int F_PollQuestion_Id, int MaxKeyValue,Entities db)
        {
         
                PollAnswer pol = new PollAnswer();
                pol.Text = AnswerText;
                pol.F_PollQuestionID = F_PollQuestion_Id;
                pol.AnswerKey = MaxKeyValue + 1;
                pol.Score = 1;
                db.PollAnswer.Add(pol);
                db.SaveChanges();
            
        }

        /// <summary>
        /// In tabe jahate virayeshe gozine haye yek soal bekar borde mishavad
        /// </summary>
        /// <param name="model">type="PollQuestionModel"</param>
        /// <returns>
        /// int
        /// </returns>
        public int EditPollAnswer(PollQuestionDataModel model, Entities db)
        {
           
                // tamamiye pasokh haye marbut be soale morede nazar ra miyabim
                var EditObject = db.PollAnswer.Where(u => u.F_PollQuestionID == model.ID);
                int MaxKeyValue;
                if (EditObject != null)
                {
                    foreach (var item in model.PollAnswer)
                    {
                        // tataboghe pasokh haye mojud dare model va pasokh haye yafte shode dar Database
                        //in kar baraye in ast ke befahmim aya pasokh feli dar halghe virayeshe pasokhist ke ghablan vojud dashte ya taze ezafe shode
                        var found = EditObject.FirstOrDefault(t => t.ID == item.ID);
                        if (found != null)
                        {
                            found.Text = item.Text;
                            found.Color = item.Color;
                        }
                        else
                        {
                            //bishtarin key valu ra dar beyne pasokh haye feli miyabim ta gozineye jadid ra ba keyvalue ye yeki bishtar az aan varede Database konim
                            MaxKeyValue = EditObject.Max(u => u.AnswerKey) ?? default(int);
                            //ba estefade az tabee farakhani shode pasokhe jadid ra ezafe mikonim
                            AddPollAnswerEditHelper(item.Text, model.ID, MaxKeyValue,db);
                        }
                    }
                    db.SaveChanges();
                    return 1;
                }
                else
                    return 2;
            
        }

 /// <summary>
 /// حذف گزینه های یک نظر سنجی
 /// </summary>
 /// <param name="PollAnswerId"></param>
 /// <param name="PollQuestionID"></param>
 /// <param name="F_username"></param>
 /// <param name="db"></param>
 /// <returns></returns>
        public string DeletePollAnswer(int PollAnswerId, int PollQuestionID,string F_username, Entities db)
        {
            db.Configuration.LazyLoadingEnabled = true;
            var _Object = db.PollQuestion.Include(t => t.PollAnswer).FirstOrDefault(u => u.ID == PollQuestionID && u.F_UserID == F_username );
                if (_Object == null) { return "NotFound"; }
                //baraye barresiye inke soale morede nazar hatman 2 gozine dashte bashad va ejazeye pak kardan do gozineye baghimande hich gah dade nemishavad
                if (_Object.PollAnswer.Count < 3) { return "MinCount"; }
                var DeleteObject = _Object.PollAnswer.FirstOrDefault(u => u.ID == PollAnswerId);
                if (DeleteObject == null) { return "NotFound"; }
                //in ghesmat baraye pak kardane PollLog haye marbut be answere morede nazar baraye pak kardan bekar miravad
                for (int i = 0; i < DeleteObject.PollLog.Count; i++)
                {
                    db.PollLog.Remove(DeleteObject.PollLog.ToList()[i]);
                }
                db.SaveChanges();
                db.PollAnswer.Remove(DeleteObject);
                db.SaveChanges();
                return "OK";
            
        }

        /// <summary>
        /// Makhsuse Web
        /// In tabe baraye bala bordane Score pasokhe morede nazar bekar miravad
        /// hengame nazar dehi tavassote karbar gozineye entekhabiyash be in method ersal mishavad
        /// ba har afzayeshe score bayad PollLog i marbut be pasokh ham sakhte shavad.
        /// </summary>
        /// <param name="PollAnswerId">type="int"</param>
        /// <returns>
        /// int
        /// </returns>
        //public int IncreasePollAnswerScore(int PollAnswerId, string F_username, Entities db)
        //{
         
        //        var IsFound = db.PollAnswer.FirstOrDefault(u => u.ID == PollAnswerId);
        //        if (IsFound != null)
        //        {
        //            IsFound.Score = IsFound.Score + 1;
        //            db.SaveChanges();
        //            PollLogManagement PLM = new PollLogManagement();
        //            PLM.AddPollLog(PollAnswerId, "Web",db);
        //            return 1;
        //        }
        //        else
        //            return 2;
            
        //}
        /// <summary>
        /// Makhsuse Android
        /// In tabe baraye bala bordane Score pasokhe morede nazar bekar miravad
        /// hengame nazar dehi tavassote karbar gozineye entekhabiyash be in method ersal mishavad
        /// ba har afzayeshe score bayad PollLog i marbut be pasokh ham sakhte shavad.
        /// </summary>
        /// <param name="PollAnswerId">type="int"</param>
        /// <returns>
        /// int
        /// </returns>
        //public int AndroidIncreasePollAnswerScore(int PollAnswerId,Entities db)
        //{
           
        //        var IsFound = db.PollAnswer.FirstOrDefault(u => u.ID == PollAnswerId);
        //        if (IsFound != null)
        //        {
        //            IsFound.Score = IsFound.Score + 1;
        //            db.SaveChanges();
        //            PollLogManagement PLM = new PollLogManagement();
        //            PLM.AddPollLog(PollAnswerId, "Android",db);
        //            return 1;
        //        }
        //        else
        //            return 2;
            
        //}



        //public List<PollAnswerModel> ListPollAnswer()
        //{
        //    using (BigMill.Models.ShahrdarEntities db = new ShahrdarEntities())
        //    {
        //        var ListObject = db.PollAnswers;
        //        List<PollAnswerModel> list = new List<PollAnswerModel>();
        //        foreach (var ListItem in ListObject)
        //        {
        //            PollAnswerModel t = new PollAnswerModel();
        //            t.AnswerKey = ListItem.AnswerKey ?? default(int);
        //            t.AnswerText = ListItem.AnswerText;
        //            t.F_PollQusetion_Id = ListItem.F_PollQuestion_Id ?? default(int);
        //            t.Score = ListItem.Score ?? default(int);
        //            t.ID = ListItem.ID;
        //            list.Add(t);
        //        }
        //        return list;
        //    }
        //}
        //public List<PollAnswerModel> ListQusetionsSpecificPollAnswer(int F_PollQuestion_Id)
        //{
        //    using (BigMill.Models.ShahrdarEntities db = new ShahrdarEntities())
        //    {
        //        var ListObject = db.PollAnswers.Where(u => u.F_PollQuestion_Id == F_PollQuestion_Id);
        //        List<PollAnswerModel> list = new List<PollAnswerModel>();
        //        foreach (var ListItem in ListObject)
        //        {
        //            PollAnswerModel t = new PollAnswerModel();
        //            t.AnswerKey = ListItem.AnswerKey ?? default(int);
        //            t.AnswerText = ListItem.AnswerText;
        //            t.F_PollQusetion_Id = ListItem.F_PollQuestion_Id ?? default(int);
        //            t.Score = ListItem.Score ?? default(int);
        //            t.ID = ListItem.ID;
        //            list.Add(t);
        //        }
        //        return list;
        //    }
        //}
    }
}