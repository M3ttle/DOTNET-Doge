using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Models.ViewModels;
using DOGEOnlineGeneralEditor.Utilities;
using System.Web.Mvc;

namespace DOGEOnlineGeneralEditor.Services
{
    public class GeneralService
    {
        private readonly IAppDataContext database;

        public GeneralService(IAppDataContext context)
        {
            database = context ?? new ApplicationDbContext();
        }
        #region FileService
        public File getFileById(int id)
        {
            var result = (from x in database.File
                          where x.ID == id
                          select x).SingleOrDefault();
            return result;
        }

        /// <summary>
        /// Function that returns true if the project with the given projectID contains a file
        /// whose filename is fileName.
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="fileName"></param>
        /// <returns>bool</returns>
        public bool fileExists(int projectID, string fileName)
        {
            File file = (from x in database.File
                         where x.Name == fileName
                         && x.Project.ID == projectID
                         select x).SingleOrDefault();
            if (file != null)
            {
                return true;
            }
            return false;
        }

		public void addFileToDatabase(File file)
		{
			database.File.Add(file);
			database.SaveChanges();
		}


		#endregion
		/// <summary>
		/// Function that returns true if a user with the given Id has access to a project
		/// whose name is projectName
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="projectName"></param>
		/// <returns></returns>
		#region ProjectService
		public bool projectExists(string userName, string projectName)
        {
            UserViewModel user = getUserByUserName(userName);
            return projectExists(user.UserID, projectName);
        }
        public bool projectExists(int userID, string projectName)
        {
            Project project = (from x in database.UserProject
                               where x.Project.Name == projectName
                               && x.UserID == userID
                               select x.Project).SingleOrDefault();
            if(project != null)
            {
                return true;
            }
            return false;
        }

        
        public void addProjectToDatabase(ProjectViewModel model)
        {
            int ownerID = getUserIDByName(model.Owner);
            Project project = new Project
            {
                Name = model.Name,
                OwnerID = ownerID,
                DateCreated = model.DateCreated,
                LanguageTypeID = model.LanguageTypeID
            };
            database.Project.Add(project);
            database.SaveChanges();
        }
        public MyProjectsViewModel getMyProjectsByName(string userName)
        {
            int userID = getUserIDByName(userName);

            var OwnedProjects = getOwnedProjects(userID);
            List<ProjectViewModel> OwnedProjectsViewModels = new List<ProjectViewModel>();
            foreach (var project in OwnedProjects)
            {
                var projectVieModel = convertProjectToViewModel(project);
                OwnedProjectsViewModels.Add(projectVieModel);
            }

            var CollaborationProjects = getCollaborationProjects(userID);
            List<ProjectViewModel> CollaboartionProjectsViewModel = new List<ProjectViewModel>();
            foreach (var project in CollaborationProjects)
            {
                var projectVieModel = convertProjectToViewModel(project);
                CollaboartionProjectsViewModel.Add(projectVieModel);
            }

            MyProjectsViewModel model = new MyProjectsViewModel
            {
                MyProjects = OwnedProjectsViewModels,
                CollaborationProjects = CollaboartionProjectsViewModel
            };

            return model;
        }

        #endregion

        #region Private ProjectService
        private List<Project> getOwnedProjects(int userID)
        {
            var result = (from p in database.Project
                          join up in database.UserProject
                          on p.ID equals up.ProjectID
                          where up.UserID == userID
                          && p.OwnerID == userID
                          select p).ToList();
            return result;
        }
        private List<Project> getCollaborationProjects(int userID)
        {
            var result = (from p in database.Project
                          join up in database.UserProject
                          on p.ID equals up.ProjectID
                          where up.UserID == userID
                          && p.OwnerID != userID
                          select p).ToList();
            return result;
        }
        private ProjectViewModel convertProjectToViewModel(Project project)
        {
            string userName = getUserNameByID(project.OwnerID);
            ProjectViewModel model = new ProjectViewModel
            {
                ProjectID = project.ID,
                Name = project.Name,
                Owner = userName,
                DateCreated = project.DateCreated
            };

            return model;
        }
        private int getProjectIDFromViewModel(ProjectViewModel model)
        {
            int userID = getUserIDByName(model.Owner);
            var project = (from x in database.Project
                           where x.Name == model.Name
                           && x.OwnerID == userID
                           select x).SingleOrDefault();
            return project.ID;
        }
        
        #endregion

        #region UserService
        /// <summary>
        /// Function that returns a viewmodel of a user from the database. 
        /// If a user is not found it throws UserNotFoundException.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>UserViewModel</returns>
        public UserViewModel getUserbyID(int userID)
        {
            User user = (from x in database.User
                        where x.ID == userID
                        select x).SingleOrDefault();
            if(user == null)
            {
                throw new UserNotFoundException();
            }
            return new UserViewModel
            {
                UserID = user.ID,
                UserName = user.Name
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns>UserViewModel</returns>
        public UserViewModel getUserByUserName(string username)
        {
            User user = (from x in database.User
                         where x.Name == username
                         select x).SingleOrDefault();
            if(user == null)
            {
                throw new UserNotFoundException();
            }
            return new UserViewModel
            {
                UserID = user.ID,
                UserName = user.Name
            };
        }
        /// <summary>
        /// Function that adds a user to the database 
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns>bool</returns>
        public void createUser(RegisterViewModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                DateCreated = DateTime.Now,
                Gender = model.Gender,
                UserTypeID = model.UserTypeID
            };

            database.User.Add(user);
            database.SaveChanges();
        }

        /// <summary>
        /// Function that returns true if a User with a given username exists in the database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>bool</returns>
        public bool userExists(string name)
        {
            User user = (from x in database.User
                         where x.Name == name
                         select x).SingleOrDefault();
            if(user != null)
            {
                return true;
            }
            return false;
        }

        public bool userProjectExists(int userID, int projectID)
        {
            UserProject userProject = (from x in database.UserProject
                                       where x.UserID == userID
                                       && x.ProjectID == projectID
                                       select x).SingleOrDefault();
            if(userProject != null)
            {
                return true;
            }
            return false;
        }
        public int getUserIDByName(string userName)
        {
            int id = (from x in database.User
                      where x.Name == userName
                      select x.ID).SingleOrDefault();
            return id;
        }

        public string getUserNameByID(int userID)
        {
            string name = (from x in database.User
                           where x.ID == userID
                           select x.Name).SingleOrDefault();
            return name;
        }
        public void addUserToProject(string userName, ProjectViewModel project)
        {
            int userID = getUserIDByName(userName);
            int projectID = getProjectIDFromViewModel(project);
            UserProject userProject = new UserProject
            {
                UserID = userID,
                ProjectID = projectID
            };
            if(userProjectExists(userID, projectID))
            {
                // duplicateexception
            }
            database.UserProject.Add(userProject);
            database.SaveChanges();
        }
        #endregion


        #region HelperFunctions
        public List<SelectListItem> getGenders()
        {
            var genders = new List<SelectListItem>();
            genders.Add(new SelectListItem() { Text = "Male", Value = "Male" });
            genders.Add(new SelectListItem() { Text = "Female", Value = "Female" });
            genders.Add(new SelectListItem() { Text = "Other", Value = "Other" });
            return genders;
        }

        public List<SelectListItem> getUserTypes()
        {
            var userTypes = new List<SelectListItem>();
            // THIS NEEDS TO GET STUFF FROM THE DATABASE, THIS IS JUST FOR TESTING!!
            /*var result = (from x in database.UserType
                          where x.Name == "Student"
                          select x);
                          
            if(result == null)
            {
                database.UserType.Add(new UserType { Name = "Student" });
                database.UserType.Add(new UserType { Name = "Teacher" });
                database.UserType.Add(new UserType { Name = "Programmer" });
                database.UserType.Add(new UserType { Name = "Other" });
            }
            
            foreach(UserType type in database.UserType)
            {
                userTypes.Add(new SelectListItem() { Text = type.Name, Value = type.ID.ToString() });
            }*/

            userTypes.Add(new SelectListItem() { Text = "Student", Value = "1" });
            userTypes.Add(new SelectListItem() { Text = "Teacher", Value = "2" });
            userTypes.Add(new SelectListItem() { Text = "Programmer", Value = "3" });
            return userTypes;
        }
        #endregion
    }
}