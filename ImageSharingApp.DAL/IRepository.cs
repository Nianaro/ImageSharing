using ImageSharingApp.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharingApp.DAL
{
    public interface IRepository
    {
        IEnumerable<User> Users { get; }
        IEnumerable<Publication> Publications { get; }
        IEnumerable<Comment> Comments { get; }
        IEnumerable<ConfirmRegistration> CRs { get; }
        IEnumerable<Subscription> Subscriptions { get; }

        ImageSharingAppContext GetContext();
        void Add<T>(T entity) where T : ImageSharingApp.DAL.Entities.Entity;
        void AddCR(ConfirmRegistration CR);
        void AddPost(Publication post);
        void Delete<T>(T entity) where T : ImageSharingApp.DAL.Entities.Entity;
        void DeleteCR(ConfirmRegistration CR);
        void SaveChanges();
    }
}
