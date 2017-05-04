using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGE.Models.POCO
{
    public class UserProject
    {
        public int ID { get; set; }
        public int ApplicationUserID { get; set; }
        public int ProjectID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Project Project { get; set; }
    }
}