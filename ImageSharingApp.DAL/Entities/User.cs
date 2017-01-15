using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ImageSharingApp.DAL.Entities
{
    public class User : Entity
    {
        public string firstName { get; set; }
        public string secondName { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public bool isAdmin { get; set; }
        public string avatar { get; set; }
        public bool isBlocked { get; set; }
    }
}
