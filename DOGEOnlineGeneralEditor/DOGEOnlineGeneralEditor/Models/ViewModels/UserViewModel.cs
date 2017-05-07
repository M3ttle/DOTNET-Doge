using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGEOnlineGeneralEditor.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class UserCollabViewModel
    {
        public int? ProjectID { get; set; }
        public List<UserViewModel> Collaborators { get; set; }
        public List<UserViewModel> NotCollaborators { get; set; }
    }
}