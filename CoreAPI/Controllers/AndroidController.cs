using AutoMapper;
using CoreAPI.Models;
using CoreAPI.Models.BLL;
using CoreAPI.Models.DataModel;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;


namespace CoreAPI.Controllers
{
    public class AndroidController : ApiController
    {
          private Entities db = DBConnect.getConnection();
          private List<IComponent> _clist= new List<IComponent>();
        public AndroidController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Menu, MenuDataModel>();
                cfg.CreateMap<MenuDataModel, Menu>();
                cfg.CreateMap<Menu, MenuAndroidDataModel>();
                cfg.CreateMap<MenuAndroidDataModel, Menu>();
                cfg.CreateMap<PollQuestion, PollQuestionDataModel>();
                cfg.CreateMap<PollQuestionDataModel, PollQuestion>();
                cfg.CreateMap<ComponentSetUp, ComponentSetUpDataModel>();
                cfg.CreateMap<ComponentSetUpDataModel, ComponentSetUp>();
                cfg.CreateMap<ComponentItemSetUp, ComponentItemSetUpDataModel>();
                cfg.CreateMap<ComponentItemSetUpDataModel, ComponentItemSetUp>();
                cfg.CreateMap<Comments, CommentDataModel>();
                cfg.CreateMap<CommentDataModel, Comments>();
                cfg.CreateMap<Posts, PostDataModel>().ForMember(dest => dest.Comments1, opt => opt.MapFrom(src => src.Comments)).ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.Name));
                cfg.CreateMap<PostDataModel, Posts>().ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments1));
                cfg.CreateMap<NewsDataModel, Posts>();
                cfg.CreateMap<Posts, NewsDataModel>();
            });
        }

        /// <summary>
        /// لیست منو بر اساس پارامتر های ورودی
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("api/Android/GetMenuAndroid")]
        [ResponseType(typeof(List<MenuAndroidDataModel>))]
        public IHttpActionResult GetMenuAndroid(string username)
        {
            List<string> menuType = new List<string> { "StaticPost", "DynamicPost", "NoneStaticDynamic" };
            List<MenuAndroidDataModel> _list = new List<MenuAndroidDataModel>();
            string userId = Tools.UserID(username);
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var menuList = db.Menu.Where(u => u.isDeleted == false && u.Language=="FA" && u.Status==true && menuType.Contains(u.Type) && u.F_UserID == userId).OrderBy(u => u.Weight).ToList();
            foreach (var m in menuList)
            {
                var temp = Mapper.Map<Menu, MenuAndroidDataModel>(m);
                if (m.Type == "StaticPost" && (!string.IsNullOrEmpty(m.ActionContent) && m.ActionContent.EndsWith(".html")))
                {
                    temp.Functionality = "WebView";
                    temp.Url = Tools.ReturnPath("DetailHTMLFilePath", username) + m.ActionContent;
                }
                else if (m.Type == "DynamicPost")
                {
                    temp.Functionality = "NewsList";
                    temp.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/android/getNewsList?username=" + username + "&id=" + m.ID;
                }

                    _list.Add(temp);
            }

            return Ok(_list);
        }

        [Route("api/Android/getFirstPage")]
        [ResponseType(typeof(RootDataModel))]
        public IHttpActionResult getFirstPage(string username)
        {
            string userId = Tools.UserID(username);
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
          //  List<IComponent> lst = new List<IComponent>();
            RootDataModel rt = new RootDataModel();

            var comp = db.ComponentSetUp.Where(u => u.F_UserID == userId).OrderBy(u=>u.Weight).ToList<ComponentSetUp>();
            foreach (var component in comp)
            {
                switch (component.Component)
                {
                    case "slider":setSlider(component.ID, username);break;
                    case "RowButton":setRowButton(component.ID,username, userId);break;
                   case "ButtonGalleryRow":setButtonGalleryRow(component.ID,username,userId);break;
                   case "NewsList":setNewsList(component.ID,username,userId);break;
                   case "GalleryButtonRow":setGalleryButtonRow(component.ID,username, userId);break;
                    case "Poll":setPoll(username);break;
                   case "Diagram":setDiagram(username);break;
                    default:
                        break;
                }
            }
            rt.Root = _clist;
            return Ok(rt);
        }

        private void setDiagram(string username)
        {
            Entities ldb = DBConnect.getEnabledLazyConnection();
            Diagram diagram = new Diagram();
            string userId = Tools.UserID(username);
            var Q = ldb.PollQuestion.Include(t => t.PollAnswer).FirstOrDefault(u => u.F_UserID == userId && u.isDeleted == false && u.EndDateOnUTC >= DateTime.UtcNow && u.StartDateOnUTC<= DateTime.UtcNow );
            List<ItemModel> DItem = new List<ItemModel>();
            if (Q != null)
            {
                foreach (var i in Q.PollAnswer)
                {
                    ItemModel qitm = new ItemModel();
                    qitm.Text = i.Text;
                    qitm.Vote = i.Score ?? default(int);
                    DItem.Add(qitm);
                }
                diagram.Question = Q.Question;
                diagram.Component = "Diagram";
            }
            diagram.Item = DItem;
            _clist.Add(diagram);
        }

        private void setPoll(string username)
        {
            Entities ldb = DBConnect.getEnabledLazyConnection();
            Poll poll = new Poll();
            string userId = Tools.UserID(username);
            var Q = ldb.PollQuestion.Include(t => t.PollAnswer).FirstOrDefault(u => u.F_UserID == userId && u.isDeleted == false && u.EndDateOnUTC >= DateTime.UtcNow && u.StartDateOnUTC<= DateTime.UtcNow);

            List<ItemModel> QItem = new List<ItemModel>();
            if (Q != null)
            {
                foreach (var i in Q.PollAnswer)
                {
                    ItemModel qitm = new ItemModel();
                    qitm.ID = i.ID;
                    qitm.Text = i.Text;
                    QItem.Add(qitm);
                }
                poll.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/PollLog/AddPollingUser?";
                poll.Question = Q.Question;
                poll.Component = "Poll";
            }

            poll.Item = QItem;
            _clist.Add(poll);
        }

        private void setGalleryButtonRow(int id,string username,string userId)
        {
            GalleryButtonRow gallerybuttonrow = new GalleryButtonRow();
              var buttonItems = db.ComponentItemSetUp.Where(u => u.F_ComponentID == id).OrderBy(u => u.Weight).ToList<ComponentItemSetUp>();
              List<ItemModel> items = new List<ItemModel>();
              foreach (var item in buttonItems)
              {
                switch (item.Type)
                {
                    case "StaticPost": items.Add(setStaticPost(item, username, userId)); break;
                    case "DynamicPost": items.Add(setDynamicPost(item, username, userId)); break;
                    case "Fanbazar": items.Add(setFanbazarValue(item, username)); break;
                    default:
                        break;
                }
            }
            GalleryManagement GM = new GalleryManagement(username);
            //dynamic collectionWrapper = new
            //{
            //    Root = GM.GetGalleryByFolderNameAndorid(FolderName)
            //};
            //var output = Newtonsoft.Json.JsonConvert.SerializeObject(collectionWrapper);
            //return output;

            var imgList = GM.GetGalleryByFolderNameAndorid("*");
            //the section below should be dynamically adjusted
            ItemModel gitem;
            List<ItemModel> gitems = new List<ItemModel>();
            foreach (var img in imgList)
            {
                gitem = new ItemModel();
                gitem.Image = img.Image;
                gitem.Functionality = "Gallery";
                gitems.Add(gitem);
            }
            gallerybuttonrow.GalleryItem = gitems;
              gallerybuttonrow.ButtonItem = items;
            gallerybuttonrow.Component = "GalleryButtonRow";
              _clist.Add(gallerybuttonrow);
        }

        private void setNewsList(int _id,string username,string userId)
        {
            var NewsItems = db.ComponentItemSetUp.Where(u => u.F_ComponentID == _id ).OrderBy(u => u.Weight).FirstOrDefault();
            if (NewsItems == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }


            NewsList newslist = new NewsList();
            List<ItemModel> items = new List<ItemModel>();
            var posts = db.Posts.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Status == true &&u.F_MenuID==NewsItems.F_ItemID ).ToList().Take(7);
            foreach (var p in posts)
            {
                ItemModel itemnews = new ItemModel();
                itemnews.Content = p.Tittle;
                itemnews.ID = p.ID;
                itemnews.Date = p.CreatedOnUTC;
                if (string.IsNullOrEmpty(p.ImagePath))
                {
                    itemnews.Image = ConfigurationManager.AppSettings["AdminAddress"] + "/Content/DefaultContent/Slider/NoImage.jpg";
                }
                else
                {
                    itemnews.Image = Tools.ReturnPath("DynamicPageImages", username) + p.ImagePath;
                }
                itemnews.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/android/GetNewsDetail?username=" + username + "&id=" + p.ID;
                itemnews.Functionality = "NewsDetails";
                items.Add(itemnews);
            }

            newslist.Component = "NewsList";
            newslist.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/android/getNewsList?username=" + username + "&id=" + _id;
            newslist.Functionality = "NewsList";

            newslist.Item = items;
            _clist.Add(newslist);
        }

        private void setButtonGalleryRow(int id,string username,string userId)
        {
            ButtonGalleryRow btngallery = new ButtonGalleryRow();
            var buttonItems = db.ComponentItemSetUp.Where(u => u.F_ComponentID == id).OrderBy(u => u.Weight).ToList<ComponentItemSetUp>();
            List<ItemModel> items = new List<ItemModel>();
            foreach (var item in buttonItems)
            {
                switch (item.Type)
                {
                    case "StaticPost": items.Add(setStaticPost(item, username, userId)); break;
                    case "DynamicPost": items.Add(setDynamicPost(item, username, userId)); break;
                    case "Fanbazar": items.Add(setFanbazarValue(item, username)); break;
                    default:
                        break;
                }
            }
            GalleryManagement GM = new GalleryManagement(username);
            //dynamic collectionWrapper = new
            //{
            //    Root = GM.GetGalleryByFolderNameAndorid(FolderName)
            //};
            //var output = Newtonsoft.Json.JsonConvert.SerializeObject(collectionWrapper);
            //return output;

         var imgList=   GM.GetGalleryByFolderNameAndorid("*");
            //the section below should be dynamically adjusted
            ItemModel gitem;
            List<ItemModel> gitems = new List<ItemModel>();
            foreach(var img in imgList) {
                gitem = new ItemModel();
                gitem.Image = img.Image;
                gitem.Functionality = "Gallery";
                gitems.Add(gitem);
            }

            btngallery.GalleryItem = gitems;
            btngallery.ButtonItem = items;
            btngallery.Component = "ButtonGalleryRow";
            _clist.Add(btngallery);
        }

        private void setRowButton(int id,string username,string userId)
        {
         
            RowButton rowbutton = new RowButton();
            var rowItems = db.ComponentItemSetUp.Where(u => u.F_ComponentID == id).OrderBy(u=>u.Weight).ToList<ComponentItemSetUp>();
            List<ItemModel> items = new List<ItemModel>();
            foreach (var item in rowItems)
            {
                switch (item.Type)
                {
                    case "StaticPost": items.Add(setStaticPost(item, username, userId));break;
                    case "DynamicPost": items.Add(setDynamicPost(item, username, userId)); break;
                    case "Fanbazar":items.Add(setFanbazarValue(item, username));break;
                  

                    default:
                        break;
                }
                 /* var mnu = db.Menu.Find(item.F_ItemID);
                  if (mnu != null)
                  {
                      ItemModel itm = new ItemModel();
                      itm.Image = Tools.ReturnPath("MenuImages", Tools.UserName(username)) + mnu.Image;
                      itm.Text = mnu.Name;
                      itm.Functionality = "WebView";
                      items.Add(itm);
                  }*/
            }
            rowbutton.Item = items;
            rowbutton.Component = "RowButton";
            _clist.Add(rowbutton);
        }

        private ItemModel setDynamicPost(ComponentItemSetUp item, string username,string userId)
        {
            var menu = db.Menu.Where(u => u.F_UserID == userId && u.isDeleted == false && u.Status == true && u.Type == "DynamicPost" && u.ID == item.F_ItemID).FirstOrDefault();
            ItemModel itm = new ItemModel();
            if (menu != null)
            {
                itm.Image = Tools.ReturnPath("MenuImages", Tools.UserName(username)) + menu.Image;
                itm.Text = menu.Name;
                itm.ID = menu.ID;
                itm.F_MenuID = menu.F_MenuID??default(int);
                itm.Functionality = "NewsList";
                itm.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/android/getNewsList?username=" + username + "&id=" + item.F_ComponentID;
            }
            return itm;
        }

        private ItemModel setFanbazarValue(ComponentItemSetUp item, string username)
        {

            ItemModel itm = new ItemModel();

                itm.Image = ConfigurationManager.AppSettings["AdminAddress"] + "/Content/DefaultContent/Slider/NoImage.jpg";
                itm.Text ="فن بازار";
                itm.Functionality = "Fanbazar";
                itm.Url = ConfigurationManager.AppSettings["AdminAddress"] + "api/Android/getFirstPage=username=fanbazar";
          
            return itm;
        }

        private ItemModel setStaticPost(ComponentItemSetUp item, string username,string userId)
        {
            var mnu = db.Menu.Where(u => u.F_UserID == userId && u.Status == true && u.isDeleted == false && u.ID == item.F_ItemID && u.Type== "StaticPost" && u.ActionContent.EndsWith(".html")).FirstOrDefault();
            if (mnu==null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            ItemModel itm = new ItemModel();

            if (mnu != null)
            {
                itm.Image = Tools.ReturnPath("MenuImages", username) + mnu.Image;
                itm.Text = mnu.Name;
                itm.ID = mnu.ID;
                itm.F_MenuID = mnu.F_MenuID??default(int);
                itm.Functionality = "WebView";
                itm.Url = ConfigurationManager.AppSettings["APIAddress"]+ "api/Android/GetHtmlDetailForAndroid?profile=" +username+ "&filename=" + mnu.ActionContent;
            }
            return itm;
        }


        [Route("api/Android/GetHtmlDetailForAndroid")]
        [ResponseType(typeof(List<MenuAndroidDataModel>))]
        public IHttpActionResult GetHtmlDetailForAndroid(string profile,string filename)
        {
            HTMLFIleToString res = new HTMLFIleToString();
            res.HtmlString = Tools.GetHtmldetail("DetailHTMLFilePath", filename, profile);
            return Ok(res);
        }
        private class HTMLFIleToString
        {
            public string HtmlString { get; set; }
        }

        private void setSlider(int compid,string username)
        {
            Slider s = new Slider();
            s.Component = "slider";   
            List<ItemModel> items = new List<ItemModel>();
            SliderManagement SM = new SliderManagement(username);
            var Slides = SM.LoadSlider().Where(u => u.Display == true).OrderBy(u => u.Priority);
            foreach (var sl in Slides)
            {
                ItemModel item = new ItemModel();
                if (string.IsNullOrEmpty(sl.Img))
                {
                    item.Image = ConfigurationManager.AppSettings["AdminAddress"] + "/Content/DefaultContent/Slider/NoImage.jpg";
                }
                else
                {
                    item.Image = Tools.ReturnPath("SliderPath", username)+"/"+sl.Img;
                }
                items.Add(item);
            }
            s.Item = items;
            _clist.Add(s);
        }

        [Route("api/android/GetNewsDetail")]
        [ResponseType(typeof(PostDataModel))]
        public async Task<IHttpActionResult> GetNewsDetail(string username ,int id)//for android
        {
            string userid = Tools.UserID(username);
            var ldb = DBConnect.getEnabledLazyConnection();
            var post = await ldb.Posts.Where(u => u.isDeleted == false && u.Status==true && u.ID == id && u.F_UserID == userid).FirstOrDefaultAsync();

            if (post == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);

            }
            List<Comments> comments = post.Comments.Where(u => u.Dispaly == true).ToList<Comments>();
            post.Comments = comments;
            NewsDataModel news = new NewsDataModel();
            news = Mapper.Map<Posts, NewsDataModel>(post);
            news.Like = ConfigurationManager.AppSettings["APIAddress"]+ "api/Post/GetLike?id=" + post.ID;
            news.Dislike = ConfigurationManager.AppSettings["APIAddress"] + "api/Post/GetDisLike?id=" + post.ID;
            news.addComment = ConfigurationManager.AppSettings["APIAddress"] + "api/Comment/PostComment";
            if (string.IsNullOrEmpty(news.ImagePath))
            {
                news.ImagePath = ConfigurationManager.AppSettings["AdminAddress"] + "/Content/DefaultContent/Slider/NoImage.jpg";
            }
            else
            {
                news.ImagePath = Tools.ReturnPath("DynamicPageImages", username) + news.ImagePath;
            }
            news.Detail = Tools.GetHtmldetail("DetailHTMLFilePath", news.Detail, username);
          //  news.Detail = Tools.ReturnPath("DetailHTMLFilePath", username) + news.Detail;
            NewsList newslist = new NewsList();
            List<PostDataModel> _Postlist = new List<PostDataModel>();
            var PostList = await db.Posts.Where(u => (u.isDeleted == false && u.Status == true) && (u.Tittle.Contains(post.Tittle) || u.Description.Contains(post.Tittle) || u.Detail.Contains(post.Tittle))).ToListAsync();

            foreach (var m in PostList)
            {
                if (m.ID != post.ID)
                {
                    _Postlist.Add(Mapper.Map<Posts, PostDataModel>(m));
                }
            }

            List<ItemModel> items = new List<ItemModel>();
            foreach (var p in _Postlist)
            {
                ItemModel itemnews = new ItemModel();
                itemnews.Content = p.Tittle;
                itemnews.ID = p.ID;
                itemnews.Date = p.CreatedOnUTC;
                if (string.IsNullOrEmpty(p.ImagePath))
                {
                    itemnews.Image = ConfigurationManager.AppSettings["AdminAddress"] + "/Content/DefaultContent/Slider/NoImage.jpg";
                }
                else
                {
                    itemnews.Image = Tools.ReturnPath("DynamicPageImages", Tools.UserName(username)) + p.ImagePath;
                }
                itemnews.Image = Tools.ReturnPath("DynamicPageImages", Tools.UserName(username)) + p.ImagePath;
                itemnews.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/android/GetNewsDetail?username=" + username + "&id=" + p.ID;
                itemnews.Functionality = "NewsDetails";
                items.Add(itemnews);
            }
            newslist.Item = items;
            news.RelatedTopics = newslist;
            return Ok(news);
        }


        [Route("api/android/getNewsList")]
        [ResponseType(typeof(News_List))]
        public async Task<IHttpActionResult> getNewsList(string username,int id)
        {
            var NewsItems = db.ComponentItemSetUp.Where(u => u.F_ComponentID == id).OrderBy(u => u.Weight);
            if (NewsItems == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            List<News_List> _catlist = new List<News_List>();
            string userid = Tools.UserID(username);
    
           /* var newsdb = from mnu in db.Menu where mnu.isDeleted==false && mnu.Status==true
                         select new
                         {
                             MENUID=mnu.ID,
                             Name=mnu.Name,
                             posts=(from pst in db.Posts where pst.F_MenuID==mnu.ID && pst.isDeleted==false select new
                             {
                                 Content = pst.Description,
                                 Date = pst.CreatedOnUTC,
                                 Image = pst.ImagePath,
                                 ID = pst.ID,
                                 Text = pst.Tittle,
                             }
                             )
                         };
            foreach (var st in newsdb)
            {
                List<ItemModel> items3 = new List<ItemModel>();

                News_List newslist = new News_List();
                foreach (var pitm in st.posts)
                {
                    ItemModel item3 = new ItemModel();
                    item3.Date = pitm.Date;
                    item3.Image = pitm.Image;
                    item3.ID = pitm.ID;
                    item3.Url = "http://parkapi.3mill.ir/api/Post/GetPostDetail?id=" + pitm.ID + "&&username=" + username;
                    item3.Functionality = "NewsDetails";
                    item3.Text = pitm.Text;
                    items3.Add(item3);
                }
                newslist.Item = items3;
                newslist.Category = st.Name;
                _catlist.Add(newslist);
            }*/
            
            var menuList =await  db.Menu.Where(u => u.isDeleted == false && u.F_UserID == userid &&u.Status==true && NewsItems.Any(m=>m.F_ItemID==u.ID)).ToArrayAsync();
              foreach (var item in menuList)
              {
                  List<NewsItemModel> items3 = new List<NewsItemModel>();

                  News_List newslist = new News_List();
                  var posts = db.Posts.Where(u => u.F_MenuID == item.ID && u.isDeleted == false && u.Status == true);
                //int i = 0;
                  foreach (var st in posts)
                  {
                      NewsItemModel item3 = new NewsItemModel();
                      item3.Content = st.Tittle;
                      item3.Date = st.CreatedOnUTC;
                    if (string.IsNullOrEmpty(item3.Image))
                    {
                        item3.Image = ConfigurationManager.AppSettings["AdminAddress"] + "/Content/DefaultContent/Slider/NoImage.jpg";
                    }
                    else
                    {
                        item3.Image = Tools.ReturnPath("DynamicPageImages", username) + st.ImagePath;
                    }

                      item3.ID = st.ID;
                      item3.Url = ConfigurationManager.AppSettings["APIAddress"] + "api/android/GetNewsDetail?username=" + username + "&id=" + st.ID;
                    item3.Functionality = "NewsDetails";
                      item3.Text = st.Tittle;
                      item3.Likes = st.NumberOfLikes??default(int);
                      item3.Dislikes = st.NumberOfDislikes ?? default(int);
                      item3.Comment = st.NumberOfComments ?? default(int);
                      items3.Add(item3);
                  //  i++;
                  }
                 newslist.Item = items3;
                 newslist.Category=item.Name;
                _catlist.Add(newslist);

            }
            return Ok(_catlist);

        }
        [Route("api/android/getGallery")]
        [ResponseType(typeof(RootDataModel))]
        public IHttpActionResult getGallery(string username)
        {
            //just for test
            RootDataModel rt = new RootDataModel();
            List<IComponent> lst = new List<IComponent>();
            ItemModel item = new ItemModel();
            List<ItemModel> items = new List<ItemModel>();
            Gallery gallery1 = new Gallery();
            Gallery gallery2 = new Gallery();
            item.Image = "https://file.digi-kala.com/digikala/Image/Webstore/Banner/1396/9/22/7454956a.jpg";
            items.Add(item);
            items.Add(item);
            items.Add(item);
            gallery1.Folder = "پارک";
            gallery1.Item = items;
            gallery2.Folder = "عمومی";
            gallery2.Item = items;
            lst.Add(gallery1);
            lst.Add(gallery2);
            rt.Root = lst;
            return Ok(rt);
        }
        [Route("api/android/getCompanyList")]
        [ResponseType(typeof(RootDataModel))]
        public IHttpActionResult getCompanyList()
        {
            RootDataModel rt = new RootDataModel();
            List<IComponent> lst = new List<IComponent>();
            List<CompanyList> companies = new List<CompanyList>();
            Company company1 = new Company();
            Company company2 = new Company();
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var users = usermanager.Users.ToList();
            foreach (var item in users)
            {
                CompanyList comp = new CompanyList();
                comp.CEO = "هرقلی";
                comp.Website = "http://3mill.ir";
                comp.Name = item.UserName;
                companies.Add(comp);
            }
            company1.CompanyList = companies;
            company1.Type = "نوع اول";
            company2.CompanyList = companies;
            company2.Type = "نوع دوم";
            lst.Add(company1);
            lst.Add(company2);
            rt.Root = lst;
            return Ok(rt);


        }
        [Route("api/android/PostComponentSetUp")]
        public async Task<IHttpActionResult> PostComponentSetUp(ComponentSetUpDataModel setup)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            string userid = Tools.UserID();
            setup.F_UserID = userid;
            ComponentSetUp _setup = Mapper.Map<ComponentSetUpDataModel, ComponentSetUp>(setup);
            db.ComponentSetUp.Add(_setup);
            await db.SaveChangesAsync();
            return Ok("با موفقیت ثبت شد");
        }
        [Route("api/android/PostListComponentSetUp")]
        public async Task<IHttpActionResult> PostListComponentSetUp(List<ComponentSetUpDataModel> setup)
        {
           
            string userid = Tools.UserID();
            foreach (var item in setup)
            {
                item.F_UserID = userid;
                ComponentSetUp _setup = Mapper.Map<ComponentSetUpDataModel, ComponentSetUp>(item);
                db.ComponentSetUp.Add(_setup);
            }
            await db.SaveChangesAsync();
            return Ok("با موفقیت ثبت شد");
        }
        [Route("api/android/PutComponentSetUp")]
        public async Task<IHttpActionResult>PutComponentSetUp(ComponentSetUpDataModel setup)
        {
            if(!SetUpExists(setup.ID))
            {
                return Content(HttpStatusCode.NotFound, "تنظیمات مورد نظر پیدا نشد");
            }
            var _setup =await db.ComponentSetUp.FindAsync(setup.ID);
            Mapper.Map(setup, _setup);
            db.Entry(_setup).State = EntityState.Modified;
            db.Entry(_setup).Property(u => u.F_UserID).IsModified = false;
            await db.SaveChangesAsync();
            return Ok("با موفقیت ویرایش شد");
        }
        public async Task<IHttpActionResult>DeleteComponentSetUp(int id)
        {
            if (!SetUpExists(id))
            {
                return Content(HttpStatusCode.NotFound, "تنظیمات مورد نظر پیدا نشد");
            }
            var _setup = await db.ComponentSetUp.FindAsync(id);
            db.ComponentSetUp.Remove(_setup);
            await db.SaveChangesAsync();
            return Ok("با موفقیت حذف شد");
        }
        [Route("api/android/PostComponentItemSetUp")]
        public IHttpActionResult PostComponentItemSetUp(ComponentItemSetUpDataModel setup)
        {
            if (!SetUpExists(setup.F_ComponentID ?? default(int)))
            {
                return Content(HttpStatusCode.NotFound, "اطلاعات ورودی ناقص است");
            }
            ComponentItemSetUp _setup = Mapper.Map<ComponentItemSetUpDataModel, ComponentItemSetUp>(setup);
            db.ComponentItemSetUp.Add(_setup);
            db.SaveChanges();
            return Ok("با موفقیت ثبت شد");
        }
        [Route("api/android/PostListComponentItemSetUp")]
        public IHttpActionResult PostListComponentItemSetUp(List<ComponentItemSetUpDataModel> setup,int id)//ComponentSetUp id
        {
            if (!SetUpExists(id))
            {
                return Content(HttpStatusCode.NotFound, "اطلاعات ورودی ناقص است");
            }
            foreach (var item in setup)
            {
                ComponentItemSetUp _setup = Mapper.Map<ComponentItemSetUpDataModel, ComponentItemSetUp>(item);
                db.ComponentItemSetUp.Add(_setup);
            }
            db.SaveChanges();
            return Ok("با موفقیت ثبت شد");
        }
        [Route("api/android/PutComponentItemSetUp")]
        public async Task<IHttpActionResult> PutComponentItemSetUp(ComponentItemSetUpDataModel setup)
        {
            if (!SetUpExists(setup.F_ComponentID??default(int)))
            {
                return Content(HttpStatusCode.NotFound, "تنظیمات مورد نظر پیدا نشد");
            }
            var _setup =await db.ComponentItemSetUp.FindAsync(setup.ID);
            Mapper.Map(setup, _setup);
            db.Entry(_setup).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok("با موفقیت ویرایش شد");
        }
        [Route("api/android/DeleteComponentItemSetUp")]
        public async Task<IHttpActionResult>DeleteComponentItemSetUp(int id)
        {
            var _setup = await db.ComponentItemSetUp.FindAsync(id);
            db.ComponentItemSetUp.Remove(_setup);
            await db.SaveChangesAsync();
            return Ok("با موفقیت حذف شد");
        }
        public bool SetUpExists(int id)
        {
            string userid = Tools.UserID();
            return db.ComponentSetUp.Count(u => u.ID == id && u.F_UserID == userid) > 0;
        }




        //public string AndroidGallery(string username)
        //{

        //    //int F_UserID = Tools.F_UserId(profile);
        //    PostManagement post = new PostManagement();
        //    //var  Photos=post.AndroidPhotosGallery(profile);
        //    //string webUrl = System.Configuration.ConfigurationManager.AppSettings["WebSiteURL"];
        //    //string result = "{\"Root\": [";
        //    //foreach (var item in Photos)
        //    //{
        //    //    result += "{\"Image\": \"http://" + webUrl + "/Content/UplodedImages/PostImages/" + item + "\"},";
        //    //}
        //    //result = result.Remove(result.LastIndexOf(','), 1);
        //    //result += "]}";
        //    //return result;
        //    dynamic collectionWrapper = new
        //    {

        //        Root = post.AndroidPhotosGallery(profile)

        //    };
        //    var output = Newtonsoft.Json.JsonConvert.SerializeObject(collectionWrapper);
        //    return output;

        //}

        [Route("api/android/GetGalleryFolder")]
        [ResponseType(typeof(RootDataModel))]
        public IHttpActionResult GetGalleryFolder(string username)
        {
            FolderManagement FM = new FolderManagement(username);
            List<FolderModel> CountAndName = new List<FolderModel>();
            GalleryManagement GM = new GalleryManagement(username);
            var PhotoList = GM.UserLoadPhotos();
            if (PhotoList.Count > 0)
            {
                var count = (from c in PhotoList group c.Type by c.Type into g select new { number = g.Count(), Type = g.Key }).ToList();
                foreach (var q in count)
                {
                    CountAndName.Add(new FolderModel { FolderName = q.Type, ExistingImagesCount = q.number });
                }
            }
            //dynamic collectionWrapper = new
            //{
            //    Root = CountAndName.Select(y => new { y.FolderName, y.ExistingImagesCount })
            //};
            //var output = Newtonsoft.Json.JsonConvert.SerializeObject(collectionWrapper);
            //return output;

            return Ok(CountAndName.Select(y => new { y.FolderName, y.ExistingImagesCount }));

        }

        [Route("api/android/GetGalleryImage")]
        [ResponseType(typeof(RootDataModel))]
        public IHttpActionResult GetGalleryImage(string username, string FolderName)
        {
            GalleryManagement GM = new GalleryManagement(username);
            //dynamic collectionWrapper = new
            //{
            //    Root = GM.GetGalleryByFolderNameAndorid(FolderName)
            //};
            //var output = Newtonsoft.Json.JsonConvert.SerializeObject(collectionWrapper);
            //return output;

            return Ok(GM.GetGalleryByFolderNameAndorid(FolderName)) ;
        }
       

    }
}
