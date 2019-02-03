using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CoreAPI.Models;
using CoreAPI.Providers;
using CoreAPI.Results;
using System.Net;
using System.Web.Security;
using System.Net.Http.Headers;
using Microsoft.Owin.Security.Infrastructure;
using System.Linq;
using CoreAPI.Models.DataModel;
using AutoMapper;
using CoreAPI.Models.BLL;
using System.Data.Entity;

namespace CoreAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private Entities db = DBConnect.getConnection();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<UserInformation, UserInformationDataModel>();
                cfg.CreateMap<UserInformationDataModel, UserInformation>();
            });
        }
        [AllowAnonymous]
        [Route("GetExistUser")]
        public IHttpActionResult GetExistUser(string username, string url)
        {

            string userid = Models.BLL.Tools.UserID(username);
            if (string.IsNullOrEmpty(userid))
            {
                return Unauthorized();
            }

            var conf = db.AdminPanelConfig.FirstOrDefault(u => u.F_UserID == userid && (url == "localhost" || u.WebSiteURL.Contains(url)));
            if (conf != null)
            {
                WebsiteProfileSetting set = new WebsiteProfileSetting();
                set.WebsiteThemeLangs = conf.WebsiteThemeLangs;
                set.WebsiteTheme = conf.WebsiteTheme;
                set.WebSiteURL = conf.WebSiteURL;
                return Ok(set);
            }
            return NotFound();

        }

        [AllowAnonymous]
        [Route("GetRootUserProfile")]
        public IHttpActionResult GetRootUserProfile(string UserID = "")
        {

            if (!User.Identity.IsAuthenticated)
            {
                return Content(HttpStatusCode.NotFound, "شما اجازه دسترسی ندارید");
            }
            if (User.Identity.IsAuthenticated)
            {
                AccountAuthorizeModel _acc = new AccountAuthorizeModel();
                string UserId = string.IsNullOrEmpty(UserID) ? Tools.RootUserID() : UserID;
                _acc.UserName = Tools.RootUserName(UserId);
                _acc.Roles = UserManager.GetRoles(UserId).ToList();
                return Ok(_acc);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, "اتفاق نامعلوم");
            }

        }



        [AllowAnonymous]
        [Route("GetListUser")]
        public IHttpActionResult GetListUser(string UserID = "")
        {

            if (!User.Identity.IsAuthenticated)
            {
                return Content(HttpStatusCode.NotFound, "شما اجازه دسترسی ندارید");
            }
            if (User.Identity.IsAuthenticated)
            {
                AccountAuthorizeModel _acc = new AccountAuthorizeModel();
                string UserId = string.IsNullOrEmpty(UserID) ? Tools.RootUserID() : UserID;
                _acc.UserName = Tools.RootUserName(UserId);
                _acc.Roles = UserManager.GetRoles(UserId).ToList();
                return Ok(_acc);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, "اتفاق نامعلوم");
            }

        }
        [Route("GetListUsers")]
        public async Task<IHttpActionResult> GetListUsers()
        {
            string User_ID = Tools.RootUserID();
            List<ProfileRegisterDataModel> _users = new List<ProfileRegisterDataModel>();
            ApplicationUserManager usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var context = new ApplicationDbContext();
            var Users = db.AspNetUsers;
            var profiles = db.UserInformation.ToList();
            if (User_ID != "00d8220d-dd4f-4be8-a352-de1e764b795d")
            {
                profiles = profiles.Where(u => u.F_ParrentUserID == User_ID).ToList();
            }
            foreach (var item in Users)
            {
                ProfileRegisterDataModel prof = new ProfileRegisterDataModel();
                var profile = profiles.FirstOrDefault(u => u.F_UserID == item.Id);
                if (profile != null)
                {
                    UserInformationDataModel _profile = Mapper.Map<UserInformation, UserInformationDataModel>(profile);
                    prof.Email = item.Email;
                    prof.UserName = item.UserName;
                    prof.UserID = item.Id;
                    prof.Profile = _profile;
                    _users.Add(prof);
                }
            }

            return Ok(_users);
        }

        [AllowAnonymous]
        [Route("isAuthorized")]
        public IHttpActionResult isAuthorized()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return Content(HttpStatusCode.NotFound, "شما اجازه دسترسی ندارید");
            }
            if (User.Identity.IsAuthenticated)
            {
                AccountAuthorizeModel _acc = new AccountAuthorizeModel();
                string UserId = Tools.UserID();
                _acc.UserName = User.Identity.GetUserName();
                _acc.Roles = UserManager.GetRoles(UserId).ToList();// Roles.GetRolesForUser().ToList();
                if (_acc.Roles.Any(u => u.EndsWith("User")))
                {
                    var _user = db.UserInformation.FirstOrDefault(u => u.F_UserID == UserId);
                    if (_user != null)
                    {
                        if (!string.IsNullOrEmpty(_user.F_ParrentUserID))
                        {
                            _acc.F_ParrentUserName = Tools.UserNameFromUserId(_user.F_ParrentUserID);
                        }
                    }

                }
                return Ok(_acc);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, "اتفاق نامعلوم");
            }

        }


        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }
        [AllowAnonymous]



        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()

        {
            Request.GetOwinContext().Authentication.SignOut();
            return Ok("شما ");
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return null;
            }
            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }
            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }
            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }
        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        [System.Web.Http.Description.ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = this.UserManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.EmailConfirmed = true;
                    await UserManager.UpdateAsync(user);

                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Ok();

        }



        /// <summary>
        /// ثبت نام کاربران فن بازار
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("RegisterFanbazarUser")]
        public async Task<IHttpActionResult> RegisterFanbazarUser(RegisterfanbazarUserBindingModel model)
        {
            var foundedUser = UserManager.FindByName(model.UserName);
            if (foundedUser == null)
            {
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, EmailConfirmed = false };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                Entities db = DBConnect.getConnection();
                UserInformation _userinfo = new UserInformation();
                _userinfo.FirstName = model.FirstName;
                _userinfo.LastName = model.LastName;
                _userinfo.CodeMelli = model.CodeMelli;
                _userinfo.F_CityID = model.CityId;
                _userinfo.F_UserID = user.Id;
                _userinfo.F_ParrentUserID = Tools.UserID(model.ParrentUserName);
                db.UserInformation.Add(_userinfo);
                await db.SaveChangesAsync();
                await this.UserManager.AddToRoleAsync(user.Id, "FanbazarUser");
                // string url = System.Configuration.ConfigurationManager.AppSettings["APIAddress"] + "api/Account/ConfirmEmail?token=" + user.Id + "&Email=" + user.Email;
                // System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                //new System.Net.Mail.MailAddress("test@3mill.ir", "Web Registration"),
                //new System.Net.Mail.MailAddress(user.Email));
                // m.Subject = "Email confirmation";
                // m.Body = string.Format("Dear {0}<BR/>Thank you for your registration, please click on the below link to comlete your registration: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>", user.UserName, url);
                // m.IsBodyHtml = true;

                // System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("mail.3mill.ir");
                // smtp.Credentials = new System.Net.NetworkCredential("test@3mill.ir", "123!@#qweQWE");
                // smtp.EnableSsl = false;
                // smtp.Port = 587;
                // smtp.Send(m);
                return Ok("حساب کاربری مورد نظر با موفقیت ثبت شد,success");
            }
            else
            {
                return Ok("نام کاربری تکراری می باشد,warning");
            }
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            model.UserName = "Sahibmall";
            model.Password = "123456";
            model.RoleName = "CMSBase,Fanbazar";

            var user = new ApplicationUser() { UserName = model.UserName };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            await this.UserManager.AddToRoleAsync(user.Id, model.RoleName);

            return Ok();
        }
        [Route("ProfileRegister")]
        public async Task<IHttpActionResult> ProfileRegister(ProfileRegisterDataModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            await this.UserManager.AddToRoleAsync(user.Id, model.RoleName);
            UserInformationDataModel profile = model.Profile;
            var _profile = Mapper.Map<UserInformationDataModel, UserInformation>(profile);
            _profile.F_UserID = user.Id;
            db.UserInformation.Add(_profile);
            await db.SaveChangesAsync();
            return Ok("با موفقیت ثبت شد");

        }


        [Route("PutProfileRegister")]
        [System.Web.Http.Description.ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfileRegister(UserInformationDataModel model)
        {
            var userId = Tools.UserID();
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            UserInformation user = db.UserInformation.Where(u => u.F_UserID == userId).FirstOrDefault();
            user.F_CityID = model.F_CityID;user.FirstName = model.FirstName;user.LastName = model.LastName;user.CodeMelli = model.CodeMelli;user.Tell = model.Tell;user.Address = model.Address;
            await db.SaveChangesAsync();
            return Ok();
        }



        [Route("GetUserDetailsForFanbazarUser")]
        [System.Web.Http.Description.ResponseType(typeof(UserInformationDataModel))]
        public async Task<IHttpActionResult> GetUserDetailsForFanbazarUser()
        {
            var userId = Tools.UserID();
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            UserInformation _user = await db.UserInformation.Where(u => u.F_UserID == userId).FirstOrDefaultAsync();
            var _userfact = Mapper.Map<UserInformation, UserInformationDataModel>(_user);
            var city = await db.AddressCity.Include(u => u.AddressState).FirstOrDefaultAsync(u => u.ID == _userfact.F_CityID);
            if (city != null)
            {
                _userfact.Address = city.AddressState.Name + " > " + city.Name;
                _userfact.F_StateID = city.F_StateID ?? default(int);
            }
            _userfact.Email = await UserManager.GetEmailAsync(userId);
            var count = from a in db.Item
                        where a.F_UserID == userId
                        group a by a.Type into gp
                        select new
                        {
                            count = gp.Count(),
                            type = gp.FirstOrDefault().Type
                        };
            //  var itemCount = await db.Item.Include(u => u.Menu).Where(u => type.Contains(u.Type) && u.Menu.F_UserID == userId && u.SubmitedState == "Accepted").OrderByDescending(u => u.NumberOfVisitors).CountAsync();
            FanbazarFactDataModel fact = new FanbazarFactDataModel();
            int offerdemand = count.FirstOrDefault(u => u.type == "FanbazarOfferDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOfferDemand").count;
            fact.CompanyCount = count.FirstOrDefault(u => u.type == "Company") == null ? 0 : count.FirstOrDefault(u => u.type == "Company").count;
            fact.DemandCount = count.FirstOrDefault(u => u.type == "FanbazarDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarDemand").count + offerdemand;
            fact.OfferCount = count.FirstOrDefault(u => u.type == "FanbazarOffer") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOffer").count + offerdemand;
            _userfact.FanbazarFacts = fact;
            return Ok(_userfact);
        }


        [Route("GetUserDetailsByUserID")]
        [System.Web.Http.Description.ResponseType(typeof(UserInformationDataModel))]
        public async Task<IHttpActionResult> GetUserDetailsByUserID(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            UserInformation _user = await db.UserInformation.Where(u => u.F_UserID == userId).FirstOrDefaultAsync();
            var _userfact = Mapper.Map<UserInformation, UserInformationDataModel>(_user);
            var city = await db.AddressCity.Include(u => u.AddressState).FirstOrDefaultAsync(u => u.ID == _userfact.F_CityID);
            if (city != null)
            {
                _userfact.Address = city.AddressState.Name + " > " + city.Name;
                _userfact.F_StateID = city.F_StateID ?? default(int);
            }
            _userfact.Email = await UserManager.GetEmailAsync(userId);
            var count = from a in db.Item
                        where a.F_UserID == userId
                        group a by a.Type into gp
                        select new
                        {
                            count = gp.Count(),
                            type = gp.FirstOrDefault().Type
                        };
            //  var itemCount = await db.Item.Include(u => u.Menu).Where(u => type.Contains(u.Type) && u.Menu.F_UserID == userId && u.SubmitedState == "Accepted").OrderByDescending(u => u.NumberOfVisitors).CountAsync();
            FanbazarFactDataModel fact = new FanbazarFactDataModel();
            int offerdemand = count.FirstOrDefault(u => u.type == "FanbazarOfferDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOfferDemand").count;
            fact.CompanyCount = count.FirstOrDefault(u => u.type == "Company") == null ? 0 : count.FirstOrDefault(u => u.type == "Company").count;
            fact.DemandCount = count.FirstOrDefault(u => u.type == "FanbazarDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarDemand").count + offerdemand;
            fact.OfferCount = count.FirstOrDefault(u => u.type == "FanbazarOffer") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOffer").count + offerdemand;
            _userfact.FanbazarFacts = fact;
            return Ok(_userfact);
        }

        [Route("GetUserDetailsForAdmin")]
        [System.Web.Http.Description.ResponseType(typeof(UserInformationDataModel))]
        public async Task<IHttpActionResult> GetUserDetailsForAdmin(string username)
        {
            var userId = Tools.UserID(username);
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            UserInformation _user = await db.UserInformation.Where(u => u.F_UserID == userId).FirstOrDefaultAsync();
            var _userfact = Mapper.Map<UserInformation, UserInformationDataModel>(_user);
            var city = db.AddressCity.Find(_userfact.F_CityID);
            if (city != null)
            {
                _userfact.Address = city.AddressState.Name + " > " + city.Name;
                _userfact.F_StateID = city.F_StateID ?? default(int);
            }
            _userfact.Email = await UserManager.GetEmailAsync(userId);
            var count = from a in db.Item
                        where a.F_UserID == userId
                        group a by a.Type into gp
                        select new
                        {
                            count = gp.Count(),
                            type = gp.FirstOrDefault().Type
                        };
            //  var itemCount = await db.Item.Include(u => u.Menu).Where(u => type.Contains(u.Type) && u.Menu.F_UserID == userId && u.SubmitedState == "Accepted").OrderByDescending(u => u.NumberOfVisitors).CountAsync();
            FanbazarFactDataModel fact = new FanbazarFactDataModel();
            int offerdemand = count.FirstOrDefault(u => u.type == "FanbazarOfferDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOfferDemand").count;
            fact.CompanyCount = count.FirstOrDefault(u => u.type == "Company") == null ? 0 : count.FirstOrDefault(u => u.type == "Company").count;
            fact.DemandCount = count.FirstOrDefault(u => u.type == "FanbazarDemand") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarDemand").count + offerdemand;
            fact.OfferCount = count.FirstOrDefault(u => u.type == "FanbazarOffer") == null ? 0 : count.FirstOrDefault(u => u.type == "FanbazarOffer").count + offerdemand;
            _userfact.FanbazarFacts = fact;
            return Ok(_userfact);
        }
        [Route("GetUserDetailsUser")]
        public async Task<IHttpActionResult> GetUserDetailsUser(int id)
        {
            if (!IsExist(id))
            {
                return Content(HttpStatusCode.NotFound, "کاربر مورد نظر یافت نشد");
            }
            UserInformationDataModel userinfo = new UserInformationDataModel();
            ProfileRegisterDataModel profile = new ProfileRegisterDataModel();
            var userinformation = await db.UserInformation.FindAsync(id);
            var user = await UserManager.FindByIdAsync(userinformation.F_UserID);
            userinfo = Mapper.Map<UserInformation, UserInformationDataModel>(userinformation);
            profile.Email = user.Email;
            profile.UserName = user.UserName;
            profile.Profile = userinfo;
            return Ok(profile);
        }
        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
        private bool IsExistUser(int id)
        {
            string userid = Tools.UserID();
            return db.UserInformation.Count(u => u.ID == id && u.F_UserID == userid) > 0;
        }
        private bool IsExist(int id)//for everyone who logged in
        {
            return db.UserInformation.Count(u => u.ID == id) > 0;
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
