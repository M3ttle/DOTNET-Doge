using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models.POCO;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class FileViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public LanguageType LanguageType { get; set; }
    }
}