using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class PageConfigsModel
    {
        public bool FixedSidebar { get; set; }
        public bool SidebaronHover { get; set; }
        public bool SubmenuonHover { get; set; }
        public bool FixedTopbar { get; set; }
        public bool BoxedLayout { get; set; }
        public string Color { get; set; }
        public string Theme { get; set; }
        public string BackGround { get; set; }
    }
}