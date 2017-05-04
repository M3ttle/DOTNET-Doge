using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models;
using System.ComponentModel.DataAnnotations.Schema;
public enum UserType
{
    Student, Teacher, Programmer, Other
}
namespace DOGEOnlineGeneralEditor.Models.POCO
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime DateCreated { get; set; }
        public string Gender { get; set; }
        public UserType UserType { get; set; }
        public string Email { get; set; }
    }
}