using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CoreAPI.Models.BLL
{

    public class GalleryManagement
    {

        public GalleryManagement(string profile)
        {
            F_UserName = profile;
            FullPath = Tools.ReturnPath("GalleryPath", F_UserName);
            Path = Tools.ReturnPath("GalleryPath", F_UserName);
        }
        string FullPath { get; set; }
        string F_UserName { get; set; }
        string Path { get; set; }

        #region user

        public List<GalleryModelAndroid> GetGalleryByFolderNameAndorid(string FolderName)
        {
            string WebSiteUrl = System.Configuration.ConfigurationManager.AppSettings["AdminAddress"];
            List<GalleryModelAndroid> list = new List<GalleryModelAndroid>();
            // sub string baraye in ast ke alamate ~ az avvale masir hazf shavad ta ax ha nemayesh dade shavand
            var temp = UserLoadPhotos().Where(u => (FolderName=="*" || u.Type == FolderName)).Take(7);
            foreach (var item in temp)
            {
                GalleryModelAndroid GM = new GalleryModelAndroid();
                GM.Image = Path.TrimStart('/') + item.Type + "/" + item.Path;
                list.Add(GM);
            }
            return list;
        }

        public List<GalleryModelAdmin> UserLoadPhotos()
        {
            List<GalleryModelAdmin> OBj = new List<GalleryModelAdmin>();
            //WebClient WebClient = new WebClient();
            //string YourContent = WebClient.DownloadString(Path + F_UserName + "_Scripts.xml");

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(Path + F_UserName + "_ImagesFile.xml").Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string Cont = content.ReadAsStringAsync().Result;
                        System.IO.StringReader strReader = new System.IO.StringReader(Cont);
                        XmlSerializer serializer = new XmlSerializer(typeof(List<GalleryModelAdmin>));
                        XmlTextReader xmlReader = new XmlTextReader(strReader);
                        OBj = (List<GalleryModelAdmin>)serializer.Deserialize(xmlReader);
                        return OBj;
                    }
                }
            }
        }
        #endregion
    }


    public class GalleryModel
    {
        public int ID { get; set; }
        public int F_SubCategroy_Id { get; set; }
        public string ImgPath { get; set; }
        public string Title { get; set; }
        public string CreateDateUtcJalali { get; set; }
    }

    public class GalleryModelAndroid
    {
        public string Image { get; set; }

    }

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

    public class FolderManagement
    {
        public FolderManagement(string profile)
        {
            F_UserName = profile;
            Path = Tools.ReturnPath("GalleryPath", profile);
        }
        string Path { get; set; }
        string F_UserName { get; set; }

        #region user
        public List<string> UserLoadListFolders()
        {

            List<string> OBj = new List<string>();
            //WebClient WebClient = new WebClient();
            //string YourContent = WebClient.DownloadString(Path + F_UserName + "_Scripts.xml");

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(Path + F_UserName + "_FoldersFile.xml").Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string Cont = content.ReadAsStringAsync().Result;
                        System.IO.StringReader strReader = new System.IO.StringReader(Cont);
                        XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                        XmlTextReader xmlReader = new XmlTextReader(strReader);
                        OBj = (List<string>)serializer.Deserialize(xmlReader);
                        return OBj;
                    }
                }
            }
        }

        #endregion
    }


    public class FolderModel
    {
        public string FolderName { get; set; } //Esme Folder
        public int ExistingImagesCount { get; set; } // Tedade Tasavire Har Folder
        public string InsertedFolderName { get; set; } //dar surate virayesh Esme jadide Folder
    }
}