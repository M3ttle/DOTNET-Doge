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
        public string Name { get; set; }
        public int LanguageTypeID { get; set; }
    }

    public class CreateFileFromFileViewModel
    {
        public int ProjectID { get; set; }
        [Required]
        public HttpPostedFileBase postedFile { get; set; }
        public string data { get; set; }
        public int LanguageTypeID { get; set; }
    }
    public class FileViewModel
    {
        public int ProjectID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int LanguageTypeID { get; set; }
    }
}