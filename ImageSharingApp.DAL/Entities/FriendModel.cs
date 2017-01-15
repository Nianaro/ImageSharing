using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSharingApp.DAL.Entities
{
    public class FriendModel
    {
        public int ID { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
        public int postCount { get; set; }
        public bool isBlocked { get; set; }
    }
}
