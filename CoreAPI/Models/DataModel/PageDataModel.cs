using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoreAPI.Models.DataModel
{
    public class PageDataModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ActionContent { get; set; }
        public string Language { get; set; }


    }
}