using CoreAPI.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CoreAPI.Models.BLL
{
    public class SliderManagement
    {
        public SliderManagement(string profile)
        {
            F_UserName = profile;
            Path = Tools.ReturnPath("SliderPath", F_UserName);
        }
        string Path { get; set; }
        string F_UserName { get; set; }

        public List<SliderModel> LoadSlider()
        {

            List<SliderModel> OBj = new List<SliderModel>();
            //WebClient WebClient = new WebClient();
            //string YourContent = WebClient.DownloadString(Path + F_UserName + "_Slider.xml");

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(Path + F_UserName + "_Slider.xml").Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string Cont = content.ReadAsStringAsync().Result;
                        System.IO.StringReader strReader = new System.IO.StringReader(Cont);
                        XmlSerializer serializer = new XmlSerializer(typeof(List<SliderModel>));
                        XmlTextReader xmlReader = new XmlTextReader(strReader);
                        OBj = (List<SliderModel>)serializer.Deserialize(xmlReader);
                        return OBj;
                    }
                }
            }


            //var serializer = new XmlSerializer(typeof(List<SliderModel>));
            //var x=   System.Xml.Linq.XDocument.Parse(YourContent);
            //OBj = (List<SliderModel>)serializer.Deserialize(x);
            //return OBj;

            // List<SliderModel> OBj = new List<SliderModel>();
            //if (!System.IO.File.Exists(Path + "/" + F_UserName + "_Slider.xml"))
            //{
            //    InitiateSlider();
            //}
            //var serializer = new XmlSerializer(typeof(List<SliderModel>));
            //using (var reader = XmlReader.Create(Path + "/" + F_UserName + "_Slider.xml"))
            //{
            //    OBj = (List<SliderModel>)serializer.Deserialize(reader);
            //    return OBj;
            //}
        }
    }
}