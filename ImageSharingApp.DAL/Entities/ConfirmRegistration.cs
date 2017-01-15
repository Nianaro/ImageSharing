using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharingApp.DAL.Entities
{
    public class ConfirmRegistration 
    {
        public Guid ID { get; set; }
        
        public virtual User user { get; set; }
    }
}
