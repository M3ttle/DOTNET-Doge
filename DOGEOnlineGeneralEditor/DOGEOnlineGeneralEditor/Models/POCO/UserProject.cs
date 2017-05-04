namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class UserProject
    {
        public int ID { get; set; }
        public string ApplicationUserID { get; set; }
        public int ProjectID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Project Project { get; set; }
    }
}
