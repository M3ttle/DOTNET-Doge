using System.Collections.Generic;
namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class LanguageType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DefaultName { get; set; }
        public virtual List<File> Files { get; set; }
        public virtual List<Project> Projects { get; set; }
    }
}
