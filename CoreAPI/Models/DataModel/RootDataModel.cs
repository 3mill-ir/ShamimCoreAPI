using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class RootDataModel
    {
        
      /*  public string Component { get; set; }
        public string Question { get; set; }
        public string Url { get; set; }
        public List<Slider> Root { get; set; }*/
        
        public List<IComponent> Root { get; set; }
     
     // public IEnumerable<IComponent>Root { get; set; }
        
    }
}