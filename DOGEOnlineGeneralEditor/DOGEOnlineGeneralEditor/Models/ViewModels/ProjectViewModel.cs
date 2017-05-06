using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models.POCO;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int FileCount { get; set; }
        public int LanguageTypeID { get; set; }
        public bool IsPublic { get; set; }
        public DateTime DateCreated { get; set; }
        public List<FileViewModel> Files { get; set; }
        public LanguageType LanguageType { get; set; }
    }

    public class MyProjectsViewModel
    {
        public List<ProjectViewModel> MyProjects { get; set; }
        public List<ProjectViewModel> CollaborationProjects { get; set; }
    }

    public class PublicProjectsViewModel
    {
        public List<ProjectViewModel> PublicProjects { get; set; }
    }
}