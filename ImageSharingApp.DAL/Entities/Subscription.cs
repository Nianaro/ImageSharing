using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharingApp.DAL.Entities
{
    public class Subscription : Entity
    {
        public virtual User following { get; set; }
        public virtual User follower { get; set; }
    }
}
