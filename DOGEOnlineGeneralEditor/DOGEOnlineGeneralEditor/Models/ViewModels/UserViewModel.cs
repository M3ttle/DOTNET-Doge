using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class UserViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class UserCollaboratorViewModel
    {
        public int ProjectID { get; set; }
        public List<UserViewModel> Collaborators { get; set; }
        public List<UserViewModel> NotCollaborators { get; set; }
    }
}