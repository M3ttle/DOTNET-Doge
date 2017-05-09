using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DOGEOnlineGeneralEditor.Models.POCO;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class CreateFileViewModel
    {
        public int ProjectID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public int LanguageTypeID { get; set; }
    }

    public class CreateFileFromFileViewModel
    {
        public int ProjectID { get; set; }
        [Required]
        public HttpPostedFileBase PostedFile { get; set; }
        public string Data { get; set; }
        public int LanguageTypeID { get; set; }
    }
    public class FileViewModel
    {
        public int ProjectID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public int LanguageTypeID { get; set; }
    }

    public class EditorViewModel
    {
        public int ProjectID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string  Data { get; set; }
        public int LanguageTypeID { get; set; }
        public string UserThemeID { get; set; }
        public List<LanguageType> LanguageTypes { get; set; }
    }
}