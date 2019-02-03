using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.BLL
{
    public class PollQuestionManagement
    {



        public int AddPollQuestion(PollQuestionDataModel model,string F_UserName, Entities db)
        {
         
                // check kardane in ke tarikh e shuru va payane vurudi ba shuru va payan haye feeli tadakhol nadashte bashad.
                if (CanBeInserted(model.StartDateOnUTC ?? default(DateTime), model.EndDateOnUTC ?? default(DateTime), model.ID,F_UserName,db))
                {
                    PollQuestion InsertObject = new PollQuestion();
                    InsertObject.CreatedOnUTC = DateTime.Now;
                    InsertObject.EndDateOnUTC = model.EndDateOnUTC;
                    InsertObject.Question = model.Question;
                    InsertObject.StartDateOnUTC = model.StartDateOnUTC;
                InsertObject.isDeleted = model.isDeleted;
                    InsertObject.ChartType = model.ChartType;
                    InsertObject.F_UserID = F_UserName;
                InsertObject.isDeleted = false;
                    db.PollQuestion.Add(InsertObject);
                    db.SaveChanges();
                    model.ID = InsertObject.ID;
                    PollAnswerManagement pol = new PollAnswerManagement();
                    pol.AddPollAnswer(model,db);
                    return 1;
                }
                else
                    return 2;
            
        }

        /// <summary>
        ///baraye check kardane tadakhole zamani
        ///tabe (WithoutContact) dar in tabe farakhani mishavad
        /// </summary>
        /// <param name="start">type="DateTime"</param>
        /// <param name="end">type="DateTime"</param>
        /// <param name="ID">type="int"</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool CanBeInserted(DateTime start, DateTime end, int? ID, string F_UserName, Entities db)
        {
         
                if (start < end)
                {
                   
                    var Times = db.PollQuestion.Where(u => u.EndDateOnUTC > DateTime.Now && u.isDeleted == false && u.F_UserID == F_UserName);
                    if (Times.Count() > 0)
                    {
                        foreach (var tople in Times)
                        {
                            // baraye barrasiye inke yek nazarsanji az nazare tadakhole zamani ba khodash moghayese nashavad
                            if (tople.ID != ID)
                            {
                                DateTime teststart = new DateTime();
                                teststart = tople.StartDateOnUTC ?? default(DateTime);
                                DateTime testend = new DateTime();
                                testend = tople.EndDateOnUTC ?? default(DateTime);
                                if (WithoutContact(start, end, teststart, testend) != true)
                                    return false;
                            }
                        }
                        return true;
                    }
                    return true;
                }
                else
                    return false;
            
        }
        /// <summary>
        /// tavassote tabee CanBeInserted estefade migardad baraye moghayese tarikh ha
        /// </summary>
        /// <param name="start">type="DateTime"</param>
        /// <param name="end">type="DateTime"</param>
        /// <param name="teststart">type="DateTime"</param>
        /// <param name="testend">type="DateTime"</param>
        /// <returns>
        /// bool
        /// </returns>
        private bool WithoutContact(DateTime start, DateTime end, DateTime teststart, DateTime testend)
        {
            if (((start < teststart || start == teststart) && (end < teststart || end == teststart)) || ((start > testend || start == testend) && (end > testend || end == testend)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// In Tabe baraye virayeshe soale morede nazar be hamrahe pasokh hayash estefade migardad
        /// Tavabe (EditPollAnswer va CanBeInserted) dar in tabe farakhani mishavand
        /// </summary>
        /// <param name="model">type"PollQuestionDataModel"</param>
        /// <param name="start">type"DateTime"</param>
        /// <param name="end">type"DateTime"</param>
        /// <returns>
        /// string
        /// </returns>
        public string EditPollQuestion(PollQuestionDataModel model, string F_UserName, Entities db)
        {
      
                // check kardane inke aya tarikh haye vorudi ba tarikh haye mojud tadakhol darand ya kheyr
                if (CanBeInserted(model.StartDateOnUTC ?? default(DateTime), model.EndDateOnUTC ?? default(DateTime), model.ID,F_UserName,db))
                {
                    var EditObject = db.PollQuestion.FirstOrDefault(u => u.ID == model.ID && u.F_UserID == F_UserName && u.isDeleted == false);
                    if (EditObject == null) { return "NotFound"; }
                    EditObject.EndDateOnUTC = model.EndDateOnUTC;
                    EditObject.F_UserID = F_UserName;
                    EditObject.ChartType = model.ChartType;
                    EditObject.StartDateOnUTC = model.StartDateOnUTC;
                    EditObject.Question = model.Question;
                    PollAnswerManagement pollans = new PollAnswerManagement();
                if (pollans.EditPollAnswer(model, db) != 1) {
                    return "NOK";
                }
                    db.SaveChanges();
                    return "OK";
                }
                else
                    return "Conflict";
            
        }

        //public int DeletePollQuestion(int PollQuestionId)
        //{
        //    using (BigMill.Models.ShahrdarEntities db = new ShahrdarEntities())
        //    {
        //        var DeleteObject = db.PollQuestions.FirstOrDefault(u => u.ID == PollQuestionId);
        //        if (DeleteObject != null)
        //        {
        //            db.PollQuestions.Remove(DeleteObject);
        //            db.SaveChanges();
        //            return 1;
        //        }
        //        else
        //            return 2;
        //    }
        //}

        /// <summary>
        /// In tabe baraye taghir vaziyate soale nazar sanji bekar miravad
        /// </summary>
        /// <param name="PollQuestionId">type="int"</param>
        /// <returns>
        /// string
        /// </returns>
        public string ChangeStatusPollQuestion(int PollQuestionId, string F_UserName, Entities db)
        {
       
                var ChangeStatusObject = db.PollQuestion.FirstOrDefault(u => u.ID == PollQuestionId && u.isDeleted == false && u.F_UserID == F_UserName);

                if (ChangeStatusObject == null) { return "NotFound"; }
                //ChangeStatusObject.Status = false;
                db.SaveChanges();
                return "OK";
            
        }

        /// <summary>
        /// In tabe baraye bazgardandane soale nazarsanjiye feli be hamrahe gozine haye pasokhe aan estefade migardad
        /// </summary>
        /// <returns>
        /// UserPollModel : dar surate vujude nazarsanjiye faal
        /// null: dar surate nabude nazarsanjiye faal
        /// </returns>
        public PollQuestionDataModel UserPollHandler(string profile, Entities db)
        {
      
                // estefade az LazyLoading dar query
               
                var FindedActiveObject = db.PollQuestion.FirstOrDefault(u => u.isDeleted == false && u.F_UserID == profile && (u.StartDateOnUTC < DateTime.Now || u.StartDateOnUTC == DateTime.Now) && (u.EndDateOnUTC > DateTime.Now || u.EndDateOnUTC == DateTime.Now));

                if (FindedActiveObject != null)
                {
                PollQuestionDataModel model = new PollQuestionDataModel();
                    model.Question = FindedActiveObject.Question;
                    model.ID = FindedActiveObject.ID;
                    model.ChartType = FindedActiveObject.ChartType;
                    var Answers = FindedActiveObject.PollAnswer.ToList();
                foreach (var item in Answers)
                {
                    PollAnswerDataModel PAL = new PollAnswerDataModel();
                    PAL.Text = item.Text;
                    PAL.ID = item.ID;
                    PAL.Color = item.Color;
                    PAL.Score = item.Score ?? default(int);
                    PAL.AnswerKey = item.AnswerKey ?? default(int);
                    //model.AnswerBox.Add(PAL);
                    model.PollAnswer.Add(PAL);
                }
            
                    return model;
                }
                else
                    return null;
            
        }

        //Liste tamamiye nazarsanjihaye ta behal sabt shode (Makhsuse admin)
        public List<PollQuestionDataModel> ListPollQuestion(string F_Username, Entities db)
        {
        
                // status==false yani admin aan nazarsanji ra paak karde va niazi be nemayeshe aan nist
                var ListObject = db.PollQuestion.Where(m => m.isDeleted == false && m.F_UserID == F_Username).OrderByDescending(m => m.StartDateOnUTC);
                List<PollQuestionDataModel> list = new List<PollQuestionDataModel>();
                foreach (var ListItem in ListObject)
                {
                    PollQuestionDataModel t = new PollQuestionDataModel();
             
                    t.CreatedOnUTC = ListItem.CreatedOnUTC ?? default(DateTime);
            
                    t.EndDateOnUTC = ListItem.EndDateOnUTC ?? default(DateTime);
              
                    t.F_UserID = ListItem.F_UserID ?? default(string);
                    t.Question = ListItem.Question;
                  //  t.Status = ListItem.Status ?? default(bool);
                    t.ID = ListItem.ID;
                    t.ChartType = ListItem.ChartType;
                    // baraye check kardane Faal budan ya nabudan
                    if ((ListItem.StartDateOnUTC < DateTime.Now || ListItem.StartDateOnUTC == DateTime.Now) && (ListItem.EndDateOnUTC > DateTime.Now || ListItem.EndDateOnUTC == DateTime.Now))
                        t.Active = true;
                    else
                        t.Active = false;
                    list.Add(t);
                }
                return list;
            
        }



        /// <summary>
        /// In tabe baraye bazgardandane Joziyate soale nazarsanjiye feli be hamrahe gozine haye pasokhe aan estefade migardad (Makhsuse admin)
        /// </summary>
        /// <returns>
        /// UserPollModel : dar surate vujude nazarsanjiye faal
        /// null: dar surate nabude nazarsanjiye faal
        /// </returns>
        public PollQuestionDataModel PollQuestionDetail(int PollQuestionId, string F_Username, Entities db)
        {
        
                var Founded = db.PollQuestion.FirstOrDefault(u => u.ID == PollQuestionId && u.F_UserID == F_Username && u.isDeleted == false);
                if (Founded == null || Founded.isDeleted == false) { return null; }
                PollQuestionDataModel t = new PollQuestionDataModel();
          
              
            t.CreatedOnUTC = Founded.CreatedOnUTC ?? default(DateTime);

            t.EndDateOnUTC = Founded.EndDateOnUTC ?? default(DateTime);
            DateTime end = new DateTime();
                end = Founded.EndDateOnUTC ?? default(DateTime);
          
          
                t.F_UserID = Founded.F_UserID ?? default(string);
                t.Question = Founded.Question;
                t.Status = Founded.isDeleted ?? default(bool);
                t.ID = Founded.ID;
                t.ChartType = Founded.ChartType;
                var answerbox = db.PollAnswer.Where(u => u.F_PollQuestionID == PollQuestionId);
                foreach (var item in answerbox)
                {
                PollAnswerDataModel model = new PollAnswerDataModel();
                    model.Text = item.Text;
                    model.AnswerKey = item.AnswerKey ?? default(int);
                    model.ID = item.ID;
                    model.Color = item.Color;
                    model.Score = item.Score ?? default(int);
                    model.F_PollQuestionID = item.F_PollQuestionID ?? default(int);
                    model.Score = item.Score ?? default(int);
                    t.PollAnswer.Add(model);
                }
                //Gozine hara betartibe vared shode morattab mikonad
                t.PollAnswer = t.PollAnswer.OrderBy(r => r.AnswerKey).ToList();
                // baraye check kardane Faal budan ya nabudan
                if ((Founded.StartDateOnUTC < DateTime.Now || Founded.StartDateOnUTC == DateTime.Now) && (Founded.EndDateOnUTC > DateTime.Now || Founded.EndDateOnUTC == DateTime.Now))
                    t.Active = true;
                else
                    t.Active = false;
                return t;
            
        }
    }
}