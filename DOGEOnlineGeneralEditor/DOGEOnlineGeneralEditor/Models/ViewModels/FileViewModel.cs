using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models.POCO;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class CreateFileViewModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
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