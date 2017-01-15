using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ImageSharingApp.DAL.Entities
{
    public class Publication : Entity
    {
        public DateTime date { get; set;}
        public string photo { get; set; }
        public string description { get; set; }
        public bool isBlocked { get; set; }

        public virtual IEnumerable<Comment> comments { get; set; }
        public virtual User user { get; set; }

    }
}
