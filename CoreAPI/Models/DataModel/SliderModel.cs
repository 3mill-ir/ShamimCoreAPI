using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class SliderModel
    {
        public int ID { get; set; }
        public int Priority { get; set; }
        public bool Display { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public string Link { get; set; }
    }
}