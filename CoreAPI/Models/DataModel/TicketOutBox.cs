//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoreAPI.Models.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class TicketOutBox
    {
        public int ID { get; set; }
        public string Content_One { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public string SMSText { get; set; }
        public string SMSStatusOne { get; set; }
        public string SMSStatusTwo { get; set; }
        public Nullable<System.DateTime> CreatedOnUTC { get; set; }
        public Nullable<int> F_TicketInboxID { get; set; }
        public string UserRole { get; set; }
    
        public virtual TicketInbox TicketInbox { get; set; }
    }
}
