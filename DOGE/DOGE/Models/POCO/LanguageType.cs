using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGE.Models.POCO
{
    public class LanguageType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}