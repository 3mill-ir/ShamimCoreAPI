using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace CoreAPI.Models.DataModel
{
    public class ShekayatListDataModel
    {
        public ShekayatListDataModel()
        {
            Shekayats = new List<ShekayatModel>();
        }
        public List<ShekayatModel> Shekayats { get; set; }
        public int Total { get; set; }
    }
}