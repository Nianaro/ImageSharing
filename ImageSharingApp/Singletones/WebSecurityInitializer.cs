using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

using ImageSharingApp.DAL;
using ImageSharingApp.DAL.Entities;

namespace ImageSharingApp.Singletones
{
    public class WebSecurityInitializer
    {
        private WebSecurityInitializer() { }
        public static readonly WebSecurityInitializer Instance = new WebSecurityInitializer();
        private bool isNotInit = true;
        private readonly object SyncRoot = new object();
        public void EnsureInitialize()
        {
            if (isNotInit)
            {
                lock (this.SyncRoot)
                {
                    if (isNotInit)
                    {
                        isNotInit = false;
                        /*Repository repo = new Repository();
                        IEnumerable<User> users = repo.Users;
                        var query = (from user in users select user).First();*/
                        WebSecurity.InitializeDatabaseConnection("ImageSharingAppContext", "Users", "ID", "email", false);
                    }
                }
            }
        }
    }
}