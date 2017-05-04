using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DOGE.Models.POCO
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int FileCount { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime DateCreated { get; set; }
        public ApplicationUser Owner { get; set; }
        public bool IsPublic { get; set; }
        public int LanguageTypeID { get; set; }
        public LanguageType LanguageType { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<UserProject> UserProject { get; set; }
    }
}