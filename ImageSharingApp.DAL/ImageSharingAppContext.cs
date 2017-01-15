using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using ImageSharingApp.DAL.Entities;
using System.Data.SqlClient;


namespace ImageSharingApp.DAL
{
    public class ImageSharingAppContext : DbContext
    {
        public ImageSharingAppContext()
            : base("ImageSharingAppConnection")
        { }

        public DbSet<User> users { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Publication> publications { get; set; }
        public DbSet<ConfirmRegistration> CRs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
