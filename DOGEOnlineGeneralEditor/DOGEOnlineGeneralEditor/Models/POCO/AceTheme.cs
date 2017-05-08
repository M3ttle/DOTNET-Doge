using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class AceTheme
    {
        public string ID { get; set; }
        public string Theme { get; set; }
        public List<User> Users { get; set; }
    }
}