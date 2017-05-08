using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models;
using System.ComponentModel.DataAnnotations.Schema;
public enum Gender
{
    Male, Female, Other
}
namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public Gender Gender { get; set; }
        public string AceThemeID { get; set; }
        public AceTheme AceTheme { get; set; }
        public int UserTypeID { get; set; }
        public UserType UserType { get; set; }
        public string Email { get; set; }
        public List<UserProject> UserProjects { get; set; }
    }
}