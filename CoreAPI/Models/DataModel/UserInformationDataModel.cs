using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class UserInformationDataModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CodeMelli { get; set; }
        public string Address{ get; set; }
        public string Tell { get; set; }
        public string Email { get; set; }
        public int F_CityID { get; set; }
        public int F_StateID { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public FanbazarFactDataModel FanbazarFacts { get; set; }
    }
}