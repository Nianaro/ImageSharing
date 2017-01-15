using ImageSharing.Business.Helpers;
using ImageSharingApp.DAL;
using ImageSharingApp.DAL.Entities;
using ImageSharingApp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageSharingApp.Controllers
{
    [HandleError()]
    [Culture]
    public class AdminController : Controller
    {
        AdminHelper helper = new AdminHelper(new Repository());

        public ActionResult Admin()
        {
            IEnumerable<User> users = helper.GetUsers();
            return View("Admin", users);
        }

        public ActionResult BlockOrUnblockUser(int userId)
        {
            helper.BlockOrUnblockUser(userId);
            return View("Admin");
        }

        public ActionResult UserPosts(int userId)
        {
            HttpContext.Session["userId"] = userId;
            IEnumerable<Publication> posts = helper.GetPostsByUserId(userId);
            return View("UserPosts", posts);
        }

        public ActionResult BlockOrUnblockPost(int postId)
        {
            helper.BlockOrUnblockPost(postId);
            return View("UserPosts");
        }

        public ActionResult SearchUserBySecondName(string secondName)
        {
            IEnumerable<User> users = helper.SearchUserBySecondName(secondName);
            return View("Admin", users);
        }

        public ActionResult GetCommentsByPostId(int postId)
        {
            IEnumerable<Comment> comments = helper.GetCommentsByPostId(postId);
            return View("Comments", comments);
        }

        public ActionResult SearchPostsByDate(Nullable<DateTime> date)
        {
            if (!date.HasValue) {
                return View("UserPosts");
            }
            IEnumerable<Publication> posts = helper.SearchPostsByDate(date.Value);
            return View("UserPosts", posts);
        }

        public ActionResult BlockOrUnblockComment(int commentId)
        {
            helper.BlockOrUnblockComment(commentId);
            return View("Comments");
        }
            
    }
}
