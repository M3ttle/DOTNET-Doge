﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int FileCount { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime DateCreated { get; set; }
        public int OwnerID { get; set; }
        public bool IsPublic { get; set; }
        public int LanguageTypeID { get; set; }
        public virtual LanguageType LanguageType { get; set; }
        public virtual List<File> Files { get; set; }
        public virtual List<UserProject> UserProjects { get; set; }
        
    }
}
