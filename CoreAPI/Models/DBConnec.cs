using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models
{
    public class DBConnect
    {
        private static Entities db;
        private static Entities lazydb;
        public static Entities getConnection()
        {
            if (db == null)
            {
                db = new Entities();
                db.Configuration.LazyLoadingEnabled = false;
            }
            return db;
        }
        public static Entities getEnabledLazyConnection()
        {
            if (lazydb == null)
            {
                lazydb = new Entities();
                lazydb.Configuration.LazyLoadingEnabled = true;
            }
            return lazydb;
        }
    }
}