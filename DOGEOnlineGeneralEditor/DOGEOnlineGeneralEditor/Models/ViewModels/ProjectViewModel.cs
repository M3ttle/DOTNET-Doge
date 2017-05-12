using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models.POCO;
using System.ComponentModel.DataAnnotations;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class ProjectViewModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be between 3-30 characters long.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Owner { get; set; }
        public int LanguageTypeID { get; set; }
		[Display(Name = "Public")]
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