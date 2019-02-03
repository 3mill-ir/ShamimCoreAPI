using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class MenuDataModel
    {
      /// <summary>
      /// شناسه 
      /// </summary>
        public int ID { get; set; }
        

        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>     
        public string Description { get; set; }
        

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public double Weight { get; set; }
         

        /// <summary>
        /// وضعیت نمایش
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// زبان
        /// </summary>
        public string Language { get; set; }
  

        /// <summary>
        /// شناسه والد
        /// </summary>
        public Nullable<int> F_MenuID { get; set; }

         
        /// <summary>
        /// عکس 
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// نوع 
        /// StaticPost 
        /// DynamicPost
        /// NoneStaticDynamic
        /// LandingPage
        /// FanbazarDemand
        /// FanbazarOffer
        /// FanbazarOfferDemand
        /// FanbazarCompany
        /// MultiMedia
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// متادیتا
        /// </summary>      
        public string MetaKeywords { get; set; }

        /// <summary>
        /// متا دیتا
        /// </summary>     
        public string MetaDescription { get; set; }

        /// <summary>
        /// متا دیتا
        /// </summary>         
        public string MetaTittle { get; set; }
        

        /// <summary>
        /// متا دیتا
        /// </summary>
        public string MetaSeoName { get; set; }
      /// <summary>
      /// نمایش در فوتر
      /// </summary>
        public bool DisplayInFooter { get; set; }
         /// <summary>
         /// نمایش در سایدبار
         /// </summary>
        public bool DisplayInSidebar { get; set; }

        /// <summary>
        /// نمایش در بخش منوی وب سایت
        /// </summary>
        public bool DisplayInMenu { get; set; }

    }

    public class MenuAndroidDataModel
    {
        public string Url { get; set; }
        public string Functionality { get; set; }
        /// <summary>
        /// شناسه 
        /// </summary>
        public int ID { get; set; }


        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>     
        public string Description { get; set; }


        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public double Weight { get; set; }


        /// <summary>
        /// وضعیت نمایش
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// زبان
        /// </summary>
        public string Language { get; set; }


        /// <summary>
        /// شناسه والد
        /// </summary>
        public Nullable<int> F_MenuID { get; set; }


        /// <summary>
        /// عکس 
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// نوع 
        /// StaticPost 
        /// DynamicPost
        /// NoneStaticDynamic
        /// LandingPage
        /// FanbazarDemand
        /// FanbazarOffer
        /// FanbazarOfferDemand
        /// FanbazarCompany
        /// MultiMedia
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// متادیتا
        /// </summary>      
        public string MetaKeywords { get; set; }

        /// <summary>
        /// متا دیتا
        /// </summary>     
        public string MetaDescription { get; set; }

        /// <summary>
        /// متا دیتا
        /// </summary>         
        public string MetaTittle { get; set; }


        /// <summary>
        /// متا دیتا
        /// </summary>
        public string MetaSeoName { get; set; }
        /// <summary>
        /// نمایش در فوتر
        /// </summary>
        public bool DisplayInFooter { get; set; }
        /// <summary>
        /// نمایش در سایدبار
        /// </summary>
        public bool DisplayInSidebar { get; set; }

        /// <summary>
        /// نمایش در بخش منوی وب سایت
        /// </summary>
        public bool DisplayInMenu { get; set; }

    }
    public class ListMenudatamodel
    {
        public List<MenuDataModel> ListMenu { get; set; }
        public ListMenudatamodel()
        {
            ListMenu = new List<MenuDataModel>();
        }
    }



    public class ListMenuPostCountDataModel
    {
        public ListMenuPostCountDataModel()
        {
            MenuCount = new List<MenuPostCountDataModel>();
        }
        public string Name { get; set; }
        public List<MenuPostCountDataModel> MenuCount { get; set; }
    }
    public class MenuPostCountDataModel
    {
        /// <summary>
        /// نام منو
        /// </summary>
        public string  MenuName { get; set; }

        /// <summary>
        /// تعداد پست های منو
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// شناسه منو
        /// </summary>
        public int MenuId { get; set; }
        
    }




    public class MenuParrentDataModel
    {/// <summary>
    /// نام
    /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نوع
        /// </summary>
        public string Type { get; set; }


       /// <summary>
       /// شناسه
       /// </summary>
        public int Id { get; set; }

     public MenuParrentDataModel Menu2 { get; set; }

    }




}