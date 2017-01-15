using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageSharingApp.DAL;
using ImageSharingApp.DAL.Entities;
using System.Data.SqlClient;

namespace ImageSharing.BLL.Helpers
{
    public class UserHelper
    {
        private static string GET_FRIENDS = "SELECT Users.ID, Users.firstName, Users.secondName, COUNT(Publications.ID) AS postCount FROM ([ImageSharingAppConnection].[dbo].Subscriptions JOIN [ImageSharingAppConnection].dbo.Users ON Subscriptions.following_ID = Users.ID) LEFT JOIN [ImageSharingAppConnection].dbo.Publications ON Users.ID = Publications.user_ID WHERE follower_ID = @myId AND Users.isBlocked = 0 GROUP BY Users.ID, Users.firstName, Users.secondName;";
        private static string GET_POSTS = "SELECT pubs.date, pubs.description, pubs.ID, pubs.photo, users.ID, users.firstName, users.secondName FROM ([ImageSharingAppConnection].[dbo].Subscriptions AS subs JOIN [ImageSharingAppConnection].dbo.Users AS users ON subs.following_ID = users.ID) LEFT JOIN [ImageSharingAppConnection].dbo.Publications AS pubs ON users.ID = pubs.user_ID WHERE subs.follower_ID = @myId AND pubs.isBlocked = 0 UNION SELECT pubs.date, pubs.description, pubs.ID, pubs.photo, users.ID, users.firstName, users.secondName FROM [ImageSharingAppConnection].dbo.Publications AS pubs JOIN [ImageSharingAppConnection].dbo.Users ON pubs.user_ID = Users.ID WHERE pubs.user_ID = @myId AND pubs.isBlocked = 0 ORDER BY pubs.date DESC;";

        public UserHelper(IRepository repository)
        {
            this.repository = repository;
        }

        public IRepository repository;

        public void AddUser(User user)
        {
            repository.Add(user);
            repository.SaveChanges();
        }

        public User GetUser(string em, string pass)
        {
            IEnumerable<User> users = repository.Users;
            IEnumerable<User> res = (from user in users where user.password == pass && user.email == em && user.isBlocked == false select user);
            return res.Count() == 0 ? null : res.First();
        }

        public User GetUser(int id)
        {
            IEnumerable<User> users = repository.Users;
            return (from user in users where user.ID == id select user).First();
        }

        public User GetUserByEmail(string email)
        {
            IEnumerable<User> users = repository.Users;
            IEnumerable<User> res = (from user in users where user.email == email && user.isBlocked == false select user);
            return res.Count() == 0? null: res.First();
        }

        public void AddComment(Comment comment, int postId, int userId) 
        {
            Publication post = GetPost(postId);
            User user = GetUser(userId);
            comment.publication = post;
            comment.user = user;
            repository.Add<Comment>(comment);
            repository.SaveChanges();
        }

        public void AddCR(ConfirmRegistration CR)
        {
            repository.AddCR(CR);
            repository.SaveChanges();
        }

        public ConfirmRegistration GetCRByKey(Guid id)
        {
            IEnumerable<ConfirmRegistration> CRs = repository.CRs;
            return (from cr in CRs where cr.ID == id select cr).First();
        }

        public void RemoveFromCR(ConfirmRegistration CR)
        {
            repository.DeleteCR(CR);
            repository.SaveChanges();
        }

        public bool IsCRExist(User user)
        {
            IEnumerable<ConfirmRegistration> CRs = repository.CRs;
            if ((from cr in CRs where cr.user == user select cr).Count() == 0)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<User> GetUsers(string name)
        {
            IEnumerable<User> users = repository.Users;
            return (from user in users where user.firstName == name && user.isBlocked == false select user);
        }

        public bool isFollowing(int idUser, int myId)
        {
            IEnumerable<Subscription> subscriptions = repository.Subscriptions;
            return (from subs in subscriptions where subs.following.ID == idUser && subs.follower.ID == myId select subs).Count() == 0 ? false : true;
        }

        public void Unsubscribe(int idUser, int myId)
        {
            IEnumerable<Subscription> subscriptions = repository.Subscriptions;
            Subscription s = (from subs in subscriptions where subs.following.ID == idUser && subs.follower.ID == myId select subs).First();
            repository.Delete(s);
            repository.SaveChanges();
        }

        public void Subscribe(int idUser, int myId)
        {
            User user = GetUser(idUser);
            User my = GetUser(myId);
            Subscription s = new Subscription()
            {
                following = user,
                follower = my
            };
            repository.Add(s);
            repository.SaveChanges();
        }

        public IEnumerable<FriendModel> GetFriends(int id)
        {
            SqlParameter param = new SqlParameter("@myId", id);
            return repository.GetContext().Database.SqlQuery<FriendModel>(GET_FRIENDS, param);
        }


        public void AddPublication(Publication publication, int idUser)
        {
            User u = GetUser(idUser);
            publication.user = u;
            repository.AddPost(publication);
            repository.SaveChanges();
        }

        public IEnumerable<PostModel> GetPosts(int id)
        {
            SqlParameter param = new SqlParameter("@myId", id);
            IEnumerable<PostModel> posts = repository.GetContext().Database.SqlQuery<PostModel>(GET_POSTS, param);
            return posts.ToList();
        }

        public Publication GetPost(int postId)
        {
            IEnumerable<Publication> posts = repository.Publications;
            return (from post in posts where post.ID == postId select post).First();
        }

        public IEnumerable<Comment> GetComments(int postId)
        {
            IEnumerable<Comment> comments = repository.Comments;
            return (from comment in comments where comment.publication.ID == postId && comment.isBlocked == false orderby comment.date descending select comment).ToList();
        }

        public IEnumerable<Publication> GetMyPosts(int myId)
        {
            IEnumerable<Publication> posts = repository.Publications;
            return (from post in posts where post.user.ID == myId && post.isBlocked == false orderby post.date descending select post).ToList();
        }

        public void EditPost(string edit, int postId)
        {
            IEnumerable<Publication> posts = repository.Publications;
            Publication post = (from p in posts where p.ID == postId select p).First();
            post.description = edit;
            repository.GetContext().Entry(post).State = System.Data.Entity.EntityState.Modified;
            repository.GetContext().SaveChanges();
        }

        public void RemovePost(int postId)
        {
            Publication post = repository.Publications.Where(p => p.ID == postId).First();
            IEnumerable<Comment> comments = repository.Comments.Where(c => c.publication.ID == postId);
            foreach(var comm in comments){
                repository.Delete<Comment>(comm);
            }
            repository.Delete<Publication>(post);
            repository.SaveChanges();
        }
    }
}
