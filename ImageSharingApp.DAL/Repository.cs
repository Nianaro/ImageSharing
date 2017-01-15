using ImageSharingApp.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharingApp.DAL
{
    public class Repository : IRepository
    {
        private readonly ImageSharingAppContext context = new ImageSharingAppContext();

        public ImageSharingAppContext GetContext()
        {
            return context;
        }

        public IEnumerable<Publication> Publications
        {
            get { return context.publications; }
        }

        public IEnumerable<User> Users
        {
            get { return context.users; }
        }

        public IEnumerable<Comment> Comments
        {
            get { return context.comments; }
        }

        public IEnumerable<ConfirmRegistration> CRs
        {
            get { return context.CRs; }
        }

        public IEnumerable<Subscription> Subscriptions
        {
            get { return context.Subscriptions; }
        }

        public void Add<T>(T entity) where T : ImageSharingApp.DAL.Entities.Entity
        {
            this.context.Set<T>().Add(entity);
        }


        public void AddPost(Publication post)
        {
            this.context.Set<Publication>().Add(post);
        }

        public void AddCR(ConfirmRegistration CR) 
        {
            this.context.Set<ConfirmRegistration>().Add(CR);
        }

        public void Delete<T>(T entity) where T : ImageSharingApp.DAL.Entities.Entity
        {
            this.context.Set<T>().Remove(entity);
        }

        public void DeleteCR(ConfirmRegistration CR)
        {
            this.context.Set<ConfirmRegistration>().Remove(CR);
        }

        public void SaveChanges()
        {   
            this.context.SaveChanges();
        }
    }
}
