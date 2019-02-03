using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class ShekayatModel
    {
        public int ID { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
        public string profile { get; set; }
        public string Type { get; set; }
        public int F_ShekayatID { get; set; }
        public DateTime LastUpdateOnUtc { get; set; }
        public string TrackingCode { get; set; }
        public int CountInbox { get; set; }
        public int CountOutbox { get; set; }
        public string ShakiFirstName { get; set; }
        public string ShakiLastName { get; set; }
        public string MotashakiFirstName { get; set; }
        public string MotashakiLastName { get; set; }
        public string ShakiSemat { get; set; }
        public string MotashakiSemat { get; set; }
        public string ShakiCodeMelli { get; set; }
        public string MotashakiOstan { get; set; }
        public string NoeErtebat { get; set; }
        public string MotashakiHozeKhedmat { get; set; }
        public string ShakiTell { get; set; }
        public string ShakiMobile { get; set; }
        public string ShakiAddress { get; set; }
        public Nullable<System.DateTime> TarikheName { get; set; }
        public string ShomareNaame { get; set; }
        public string NaameMahaleMorajee { get; set; }
        public string NatijeEghdameGhabli { get; set; }
        public string F_UserID { get; set; }
        public Nullable<System.DateTime> LastUpdatedOnUTC { get; set; }
    }
}