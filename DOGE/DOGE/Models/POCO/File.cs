using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGE.Models.POCO
{
    public class File
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime DateCreated { get; set; }
        public int ProjectID { get; set; }
        public int LanguageTypeID { get; set; }
        public LanguageType LanguageType { get; set; }
        public Project Project { get; set; }
    }
}