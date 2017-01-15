using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSharingApp.Models
{
    public class LookUserModel
    {
        public int ID { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
        public string avatar { get; set; }
        public bool isFolowing { get; set; }
    }
}