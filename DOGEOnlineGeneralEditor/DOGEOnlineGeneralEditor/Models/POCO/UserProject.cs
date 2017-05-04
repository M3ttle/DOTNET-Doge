namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class UserProject
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int ProjectID { get; set; }
        public virtual User User { get; set; }
        public virtual Project Project { get; set; }
    }
}
