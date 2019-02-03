using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAPI.Models
{
    public interface IComponent
    {
        int ID { get; set; }
        string Component { get; set; }
        string Functionality { get; set; }
        string Url { get; set; }

    }
    public class NewsList : IComponent
    {
        public int ID { get; set; }
        public int MenuID { get; set; }
        public int F_MenuID { get; set; }
        public string Component { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<ItemModel> Item { get; set; }
       
    }
    public class GalleryButtonRow : IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<ItemModel> GalleryItem { get; set; }
        public ICollection<ItemModel> ButtonItem { get; set; }
    }
    public class RowButton : IComponent
    {
        public int ID { get; set; }
        public int F_MenuID { get; set; }
        public string Component { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<ItemModel> Item { get; set; }
    }
    public class Slider : IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<ItemModel> Item { get; set; }
    }
    public class ButtonGalleryRow : IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<ItemModel> ButtonItem { get; set; }
        public ICollection<ItemModel> GalleryItem { get; set; }
    }
    public class Poll : IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
      //  public string Url { get; set; }
        public string Question { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }

        public ICollection<ItemModel> Item { get; set; }

    }
    public class Diagram : IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
       
        public string Question { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<ItemModel> Item { get; set; }

    }
   /*ublic class Category
    {
        public string Name { get; set; }
        public ICollection<ItemModel> Item { get; set; }
        
    }*/
    public class News_List:IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string Functionality { get; set; }
        public ICollection<NewsItemModel> Item { get; set; }
    }
    public class NewsDataModel
    {
        public int ID { get; set; }
        public string Tittle { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string ImagePath { get; set; }
        public Nullable<int> NumberOfVisitors { get; set; }
        public Nullable<int> NumberOfComments { get; set; }
        public Nullable<int> NumberOfLikes { get; set; }
        public Nullable<int> NumberOfDislikes { get; set; }
        public Nullable<System.DateTime> CreatedOnUTC { get; set; }
        public string Language { get; set; }

        public ICollection<CommentDataModel> Comments { get; set; }
        public string Like { get; set; }
        public string Dislike { get; set; }
        public string addComment { get; set; }
        public NewsList RelatedTopics { get; set; }
    }

    public class Gallery:IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public string Functionality { get; set; }
        public string Url { get; set; }
        public string Folder { get; set; }
        public ICollection<ItemModel> Item { get; set; }
    }
    public class Company:IComponent
    {
        public int ID { get; set; }
        public string Component { get; set; }
        public string Functionality { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public ICollection<CompanyList> CompanyList { get; set; }
    }
}
