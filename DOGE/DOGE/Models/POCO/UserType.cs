using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGE.Models.POCO
{
    public class UserType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}