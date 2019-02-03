using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CoreAPI.Models.DataModel;
using System.Data.Entity;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Http;
using System.Net;

namespace CoreAPI.Models.BLL
{
    public static class Tools
    {
        public static string ReturnPathPhysicalMode(string ConfigPath, string F_UserName, string DomainAddress, string Caller)
        {
            NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("UsersFoldersPath");
            string Path = ConfigurationManager.AppSettings[DomainAddress] + string.Format(section[ConfigPath], F_UserName);
            return Path;
        }
        public static string GetHtmldetail(string ConfigPath, string FileName, string F_UserName)
        {
            string path = Tools.ReturnPathPhysicalMode(ConfigPath, F_UserName, "AdminAddress", "GetHtmldetail()");

            //WebClient WebClient = new WebClient();
            //string YourContent = WebClient.DownloadString(path + ContentFour);
            //return YourContent
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(path + FileName).Result)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (HttpContent content = response.Content)
                        {
                            return content.ReadAsStringAsync().Result;
                        }
                    }
                    else
                        return "<p class=\"text-center\">این پست حاوی محتوا نمی باشد</p>";
                }
            }

        }

        public static string ReturnPath(string ConfigPath, string F_UserName)
        {
            NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("UsersFoldersPath");
            string Path = string.Format(section[ConfigPath], F_UserName);

          Path=  ConfigurationManager.AppSettings["AdminAddress"] + Path;
            return Path;
        }

        public static string UserID()
        {
            return System.Web.HttpContext.Current.User.Identity.GetUserId();
        }

        public static string RootUserID()
        {
            var UserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
            return manager.FindById(UserID).RootUserID;
        }

        public static string RootUserName(string UserID)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext.Create()));
            return manager.FindById(UserID).UserName;
        }

        public static string UserName()
        {

            return System.Web.HttpContext.Current.User.Identity.Name;
        }
        public static string UserName(string username)
        {
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindByName(username);
            return user.UserName;
        }
        public static string UserNameFromUserId(string userId)
        {
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindById(userId);
            return user.UserName;
        }
        public static string UserID(string username)
        {
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindByName(username);
            if (user != null)
            {
                return user.Id;
            }
            else
            {
                return null;
            }
        }
        public static string UserRole()
        {
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = usermanager.FindById(Tools.UserID());
            var context = new ApplicationDbContext();
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleMngr = new RoleManager<IdentityRole>(roleStore);
            var roleid = user.Roles.FirstOrDefault().RoleId;
            return roleMngr.Roles.Where(u => u.Id == roleid).FirstOrDefault().Name;

        }
    }


public  class CascadeTools
    {
      

        public  void DeleteCascadeComments(Comments cm,Entities db)
        {

          foreach(var c in cm.Comments1)
            {
                DeleteCascadeComments(c,db);
            }
            
            db.Entry(cm).State = EntityState.Deleted;
        }
        public void DeleteCascadeAttributeGroup(AttributeGroup attribute,Entities db)
        {
            foreach (var at in attribute.AttributeGroup1)
            {
                DeleteCascadeAttributeGroup(at, db);
            }
          
            db.Entry(attribute).State = EntityState.Deleted;
        }

        public void CascadeChangeStatusMenu(Menu menu, bool status, Entities db)
        {
            foreach (var m in menu.Menu1)
            {
                CascadeChangeStatusMenu(m, status,db);
            }
            menu.Status = status;
            db.Entry(menu).State = EntityState.Modified;
        }

        public void CascadeDeleteMenu(Menu menu, Entities db)
        {
            foreach (var m in menu.Menu1)
            {
                CascadeDeleteMenu(m,db);
            }
            menu.isDeleted = true;

            foreach (var p in menu.Posts)
            {
                p.isDeleted = true;
             foreach(var c in p.Comments)
                {
                    DeleteCascadeComments(c, db);
                }
                db.Entry(p).State = EntityState.Modified;

            }
            db.Entry(menu).State = EntityState.Modified;
        }
        public void CascadeDeleteFanbazarMenu(Menu menu, Entities db)
        {
            foreach (var m in menu.Menu1)
            {
                CascadeDeleteFanbazarMenu(m, db);
            }
            menu.isDeleted = true;
   
            foreach(var item in menu.Item)
            {
                
                foreach(var galleri in item.ItemGallery)
                {
                    db.Entry(galleri).State = EntityState.Deleted;
                   
                }
                db.Entry(item).State = EntityState.Deleted;
            }
            db.Entry(menu).State = EntityState.Modified;
          

        }
    }
}