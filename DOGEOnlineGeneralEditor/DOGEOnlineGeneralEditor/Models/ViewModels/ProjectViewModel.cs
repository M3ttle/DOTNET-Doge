using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public DateTime DateCreated { get; set; }
        public List<FileViewModel> Files { get; set; }
    }

    public class MyProjectsViewModel
    {
        public List<ProjectViewModel> MyProjects { get; set; }
    }
}