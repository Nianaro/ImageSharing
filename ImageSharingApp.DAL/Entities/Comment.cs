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
    public class Comment : Entity
    {
        public DateTime date { get; set; }
        public string text { get; set; }
        public bool isBlocked { get; set; }

        public virtual Publication publication { get; set;}
        public virtual User user { get; set; }
    }
}
