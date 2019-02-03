using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class ItemDataModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<System.DateTime> CreatedDateOnUTC { get; set; }

        /// <summary>
        /// Sent
        /// Seen
        /// Review
        /// Accepted
        /// </summary>
        public string SubmitedState { get; set; }
        public Nullable<int> F_MenuID { get; set; }
        public string F_UserID { get; set; }
        public Nullable<int> NumberOfVisitors { get; set; }
        public string AdminDescription { get; set; }
        public ICollection<ItemGalleyDataModel> ItemGallery { get; set; }
        public ICollection<ItemAttributegroupDataModel> ItemAttributeGroup { get; set; }
    }

    public class ItemAttributegroupDataModel
    {
        public ICollection<ItemAttributeValuedataModel> AttributeValue { get; set; }
        public ICollection<ItemAttributegroupDataModel> AttributeGroup { get; set; }
    }

    public class ItemAttributeValuedataModel
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
    public class ItemSearchFilter
    {
        public string type { get; set; }
        public int categoryid { get; set; }
        public string keyword { get; set; }
    }
    public class PostItemDescription
    {
        public int id { get; set; }
        public string state { get; set; }
        public string description { get; set; }
    }



    public class FanbazarFactDataModel
    {
        public int UserCount { get; set; }
        public int CompanyCount { get; set; }
        public int OfferCount { get; set; }
        public int DemandCount { get; set; }
    }


    public class ItemNew
    {
        //public ItemNew()
        //{
        //    this.AttributeValue = new Collection<AttributeValueNew>();
        //}

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<System.DateTime> CreatedDateOnUTC { get; set; }
        public string SubmitedState { get; set; }
        public Nullable<int> F_MenuID { get; set; }
        public string F_UserID { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
        public int Stock { get; set; }
        public Nullable<int> NumberOfVisitors { get; set; }
        public string AdminDescription { get; set; }

    //    public Collection<AttributeValueNew> AttributeValue { get; set; }
        public  MenuNew Menu { get; set; }
    }


    public class MenuNew
    {
        public MenuNew()
        {
            this.Menu1 = new Collection<MenuNew>();
            this.AttributeGroup = new Collection<AttributeGroupNew>();
            //     this.Item = new Collection<ItemNew>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string F_UserID { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTittle { get; set; }
        public string MetaSeoName { get; set; }
        public Nullable<int> F_MenuID { get; set; }

        public Collection<MenuNew> Menu1 { get; set; }
      public MenuNew Menu2 { get; set; }
       public Collection<AttributeGroupNew> AttributeGroup { get; set; }
     //   public Collection<ItemNew> Item { get; set; }
    }


    public  class AttributeGroupNew
    {
        public AttributeGroupNew()
        {
                this.Attribute = new Collection<AttributeNew>();
            this.AttributeGroup1 = new Collection<AttributeGroupNew>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<double> Weight { get; set; }
        public string Image { get; set; }
        public Nullable<int> F_AttributeGroupID { get; set; }
        public Nullable<int> Depth { get; set; }
        public Nullable<int> F_MenuID { get; set; }

      public Collection<AttributeNew> Attribute { get; set; }
      public Collection<AttributeGroupNew> AttributeGroup1 { get; set; }

    }
     

    public  class AttributeNew
    {
        public AttributeNew()
        {
            this.AttributeValue = new Collection<AttributeValueNew>();
            this.AttributeItem1 = new Collection<AttributeItemNew>();

        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<int> F_AttributeGroupID { get; set; }
        public string ComponentType { get; set; }
        public Nullable<int> F_AttributeItemID { get; set; }
        public string Icon { get; set; }
        public string TextColor { get; set; }
        public  Nullable<bool> Required { get; set; }

        //  public AttributeGroupNew AttributeGroup { get; set; }
        public Collection<AttributeValueNew> AttributeValue { get; set; }
        public Collection<AttributeItemNew> AttributeItem1 { get; set; }
    }

    public  class AttributeValueNew
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public Nullable<int> F_AttributeID { get; set; }
        public Nullable<int> F_AttributeItemID { get; set; }
        public Nullable<int> F_ItemID { get; set; }

     //  public AttributeNew Attribute { get; set; }
       // public ItemNew Item { get; set; }
    }
    public  class AttributeItemNew
    {
        public AttributeItemNew()
        {
            this.Attribute = new Collection<AttributeNew>();
            this.AttributeValue = new Collection<AttributeValueNew>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> F_AttributeID { get; set; }

        public  Collection<AttributeNew> Attribute { get; set; }
        public  Collection<AttributeValueNew> AttributeValue { get; set; }
    }

    public class FilterItemDataModel
    {

        public int MenuId { get; set; }
        public string type { get; set; }
        public Nullable<DateTime> FromTime { get; set; }
        public Nullable<DateTime> ToTime { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string search { get; set; }
        public int Searchtype { get; set; }
        public string sortby { get; set; }
        public string profile { get; set; }

    }

    public class SubmiteState
    {
        public int id { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
    }
}