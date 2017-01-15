using ImageSharing.BLL.Helpers;
using ImageSharing.Business.Tools;
using ImageSharingApp.DAL;
using ImageSharingApp.DAL.Entities;
using ImageSharingApp.Filters;
using ImageSharingApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ImageSharingApp.Controllers
{
    [HandleError()]
    [Culture]
    public class UserController : Controller
    {
        UserHelper helper = new UserHelper(new Repository());

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(string firstName, string secondName, string email, string password, string confirmPassword, HttpPostedFileBase pic)
        {
            HttpCookie lang = HttpContext.Request.Cookies["lang"];

            if (lang != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang.Value);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang.Value);
            }

            if (Utility.IsRightRegistrationData(firstName, secondName, password, confirmPassword, email))
            {
                User ExistUser = helper.GetUserByEmail(email);

                if (ExistUser != null)
                {
                    ViewBag.Title = Resources.Resource.EmailIsUsed;
                    return View("Registration");
                }

                User user = new User()
                {
                    firstName = firstName,
                    secondName = secondName,
                    email = email,
                    password = password
                };

                if (pic.ContentLength > 0)
                {
                    if (Utility.CheckTypePhotoDuringRegistration(pic.ContentType))
                    {
                       /* string fileName = Path.GetFileName(pic.FileName);*/
                        string code = Guid.NewGuid().ToString();
                        var path = Path.Combine(Server.MapPath("~/Content/images"), code/* + fileName*/);
                        pic.SaveAs(path);
                        user.avatar = code/* + fileName*/;
                    }
                    else
                    {
                        ViewBag.Title = Resources.Resource.IncorrectPhotoExtention;
                        return View("Registration");
                    }
                }

                user.password = Utility.GetHashString(user.password);
                helper.AddUser(user);
                User usr = helper.GetUserByEmail(user.email);
                Guid key = Guid.NewGuid();
                ConfirmRegistration CR = new ConfirmRegistration()
                {
                    ID = key,
                    user = usr
                };
                helper.AddCR(CR);
                Utility.ConfirmRegistration(user.email, key);
                ViewBag.Title = Resources.Resource.LetterOnYourEmailConfirmRegistration;
                return View("Login");
            }
            ViewBag.Title = Resources.Resource.IncorrectData;
            return View("Registration");
        }

        public ActionResult Login()
        {
            User user = (User)HttpContext.Session["user"];
            if (user == null)
            {
                //Uri firstEnter = Request.UrlReferrer;
                //if (firstEnter == null)
               // {
                    return View("Login");
                //}
                //return Redirect("/");
                
            }
            return Redirect("/User/Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            HttpCookie lang = HttpContext.Request.Cookies["lang"];
            if (lang != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang.Value);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang.Value);
            }

            Utility util = new Utility();

            if (util.IsValidEmail(email) && Utility.CheckPassword(password))
            {
                User user = helper.GetUser(email, Utility.GetHashString(password));
                if (user != null && helper.IsCRExist(user))
                {
                    ViewBag.Title = Resources.Resource.ConfirmRegistration;
                    return View("Login");
                }

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(email, true);
                    HttpContext.Session["user"] = user;
                    return Redirect("/User/Account");
                }
            }
            ViewBag.Title = Resources.Resource.LoginOrPasswordAreIncorrect;
            return View("Login");
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.Session["user"] = null;
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        public ActionResult Account()
        {
            User user = (User)HttpContext.Session["user"];
            if (user == null)
            {
                return Redirect("/");
            }
            IEnumerable<PostModel> posts = helper.GetPosts(user.ID);
            return View("Account", posts);
        }

        public ActionResult ConfirmRegistration(string key)
        {
            ConfirmRegistration CR = helper.GetCRByKey(new Guid(key));
            User user = CR.user;
            HttpContext.Session["user"] = user;
            helper.RemoveFromCR(CR);
            FormsAuthentication.SetAuthCookie(user.email, true);
            return View("Account");
        }

        public ActionResult Header()
        {
            return PartialView();
        }

        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult SearchFriend(string name)
        {
            var users = helper.GetUsers(name).ToList();

            if (users.Count() == 0)
            {
                return View("SearchFriend");
            }

            return View("SearchFriend", users);
        }

        public ActionResult LookUser(int id)
        {
            var user = helper.GetUser(id);
            User u = ((User)(HttpContext.Session["user"]));
            int myId = u == null ? -1 : u.ID;
            LookUserModel LUM = new LookUserModel()
            {
                ID = user.ID,
                firstName = user.firstName,
                secondName = user.secondName,
                avatar = user.avatar,
                isFolowing = helper.isFollowing(user.ID, myId)
            };
            return View(LUM);
        }

        public ActionResult SubscrOrUnsubscr(int id)
        {
            int myId = ((User)(HttpContext.Session["user"])).ID;
            if (helper.isFollowing(id, myId))
            {
                helper.Unsubscribe(id, myId);
            }
            else
            {
                helper.Subscribe(id, myId);
            }
            return PartialView();
        }

        public ActionResult Friends()
        {
            User my = (User)HttpContext.Session["user"];
            if (my == null)
            {
                return Redirect("/");
            }
            return View("Friends", helper.GetFriends(my.ID).ToList());
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPost(string description, HttpPostedFileBase picture)
        {
            Publication publication = null;

            if (picture.ContentLength > 0 && Utility.CheckTypePhotoDuringRegistration(picture.ContentType))
            {
                string code = Guid.NewGuid().ToString();
                string fileName = Path.GetFileName(picture.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images"), code + fileName);
                picture.SaveAs(path);

                publication = new Publication()
                {
                    date = DateTime.Now,
                    description = description.Length > 255 ? description.Substring(0, 255) : description,
                    photo = code + fileName,
                };
            }
            else {
                return Redirect("/User/Account");
            }
            User usr = (User)HttpContext.Session["user"];
            helper.AddPublication(publication, usr.ID);
            return PartialView("AddPost");
        }

        [Authorize]
        public ActionResult LocatePosts()
        {
            User user = (User)HttpContext.Session["user"];
            IEnumerable<PostModel> posts = helper.GetPosts(user.ID);
            return PartialView("LocatePosts", posts);
        }

        [Authorize]
        public ActionResult GetPost(int postId)
        {
            Publication post = helper.GetPost(postId);
            HttpContext.Session["post"] = post;
            return PartialView("GetPost");
        }

        [Authorize]
        public ActionResult PopupPost()
        {
            Publication post = (Publication)HttpContext.Session["post"];
            HttpContext.Session["post"] = null;
            post.comments = helper.GetComments(post.ID);
            return PartialView("PopupPost", post);
        }

        [Authorize]
        public ActionResult AddComment(int postId, string text)//Costil
        {
            Comment comment = new Comment()
            {
                date = DateTime.Now,
                text = text.Length > 255 ? text.Substring(0, 255) : text
            };
            User user = (User)HttpContext.Session["user"];
            helper.AddComment(comment, postId, user.ID);
            IEnumerable<Comment> coments = helper.GetComments(postId);
            return PartialView("AddComment", coments);
        }

        [Authorize]
        public ActionResult GetMyPosts()
        {
            User user = (User)HttpContext.Session["user"];
            IEnumerable<Publication> posts = helper.GetMyPosts(user.ID);
            return View("MyPosts", posts);
        }

        [Authorize]
        public ActionResult EditPost(string edit, int postId)
        {
            helper.EditPost(edit.Length > 255 ? edit.Substring(0, 255) : edit, postId);
            return View();
        }

        [Authorize]
        public ActionResult RemovePost(int postId)
        {
            helper.RemovePost(postId);
            return View();
        }

        public ActionResult ChangeCulture(string lang)
        {
            // Список культур
            List<string> cultures = new List<string>() { "ru", "en" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }
            // Сохраняем выбранную культуру в куки
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;   // если куки уже установлено, то обновляем значение
            else
            {

                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            User user = (User)HttpContext.Session["user"];
            if (user == null) {
                return Redirect("/");
            }
            return Redirect("/User/Account");
        }
    }
}
