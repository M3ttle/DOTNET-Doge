using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOGEOnlineGeneralEditor.Models.POCO
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
        public virtual LanguageType LanguageType { get; set; }
        public virtual Project Project { get; set; }
    }
}
