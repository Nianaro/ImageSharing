using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharingApp.DAL.Entities
{
    public class PostModel
    {
        public Nullable<int> ID { get; set; }
        public Nullable<DateTime> date { get; set; }
        public string photo { get; set; }
        public string description { get; set; }
        public Nullable<bool> isBlocked { get; set; }

        public int userId { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
    }
}
