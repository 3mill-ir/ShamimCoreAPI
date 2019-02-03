using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class FilterPostDatamodel
    {

        public int MenuId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Language { get; set; }
        public string search { get; set; }
        public int type { get; set; }

        public string sortby { get; set; }

    }
}