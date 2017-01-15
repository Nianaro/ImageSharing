using ImageSharingApp.DAL;
using ImageSharingApp.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharing.Business.Helpers
{
    public class AdminHelper
    {
        public AdminHelper(IRepository repository)
        {
            this.repository = repository;
        }

        public IRepository repository;

        public IEnumerable<User> GetUsers()
        {
            return repository.Users.ToList();
        }

        public bool isBlockedUser(int userId)
        {
            IEnumerable<User> users = repository.Users;
            return (from user in users where user.ID == userId select user).First().isBlocked;
        }

        public bool isBlockedPost(int postId)
        {
            IEnumerable<Publication> posts = repository.Publications;
            return (from post in posts where post.ID == postId select post).First().isBlocked;
        }

        public bool isBlockedComment(int commentId)
        {
            IEnumerable<Comment> comments = repository.Comments;
            return (from comm in comments where comm.ID == commentId select comm).First().isBlocked;
        }

        public void BlockOrUnblockUser(int userId)
        {
            IEnumerable<User> users = repository.Users;
            User user = (from u in users where u.ID == userId select u).First();
            if (isBlockedUser(userId))
            {
                user.isBlocked = false;
            }
            else
            {
                user.isBlocked = true;
            }
            repository.GetContext().Entry(user).State = System.Data.Entity.EntityState.Modified;
            repository.SaveChanges();
        }

        public IEnumerable<Publication> GetPostsByUserId(int userId)
        {
            IEnumerable<Publication> posts = repository.Publications;
            return (from post in posts where post.user.ID == userId select post);
        }

        public IEnumerable<User> SearchUserBySecondName(string secondName)
        {
            IEnumerable<User> users = repository.Users.Where(u => u.secondName == secondName);
            return users.ToList();
        }

        public IEnumerable<Comment> GetCommentsByPostId(int postId)
        {
            IEnumerable<Comment> comments = repository.Comments;
            return (from comment in comments where comment.publication.ID == postId orderby comment.date descending select comment).ToList();
        }

        public IEnumerable<Publication> SearchPostsByDate(DateTime date)
        {
            DateTime border = date.Date.AddDays(1);
            IEnumerable<Publication> posts = repository.Publications;
            IEnumerable<Publication> res = (from post in posts where post.date >= date && post.date < border select post).ToList();
            return res;
        }

        public void BlockOrUnblockPost(int postId)
        {
            IEnumerable<Publication> posts = repository.Publications;
            Publication post = (from p in posts where p.ID == postId select p).First();
            if (isBlockedPost(postId))
            {
                post.isBlocked = false;
            }
            else
            {
                post.isBlocked = true;
            }
            repository.GetContext().Entry(post).State = System.Data.Entity.EntityState.Modified;
            repository.SaveChanges();
        }

        public void BlockOrUnblockComment(int commentId)
        {
            IEnumerable<Comment> comments = repository.Comments;
            Comment comment = (from comm in comments where comm.ID == commentId select comm).First();
            if (isBlockedComment(commentId))
            {
                comment.isBlocked = false;
            }
            else
            {
                comment.isBlocked = true;
            }
            repository.GetContext().Entry(comment).State = System.Data.Entity.EntityState.Modified;
            repository.SaveChanges();
        }
    }
}
