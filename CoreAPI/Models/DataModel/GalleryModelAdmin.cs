using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreAPI.Models.DataModel
{
    public class GalleryModelAdmin
    {
        public int ID { get; set; }
        //esme parvande
        public string Type { get; set; }
        public string Text { get; set; }
        //Name Tasvir
        public string Path { get; set; }
        //int type moghe avaz kardane jaye tasvir bedard mikhorad
        public string BackupType { get; set; }
    }

    public class GalleryListModel
    {
        public GalleryListModel()
        {
            Folders = new List<GalleryModelAdmin>();
        }
        public List<GalleryModelAdmin> Folders { get; set; }
        public string FolderName { get; set; }
    }
}