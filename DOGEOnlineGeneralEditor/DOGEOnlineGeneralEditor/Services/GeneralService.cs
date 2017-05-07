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

		public void addFileToDatabase(CreateFileViewModel model)
		{
            File file = new File
            {
                Name = model.Name,
                LanguageTypeID = model.LanguageTypeID,
                ProjectID = model.ProjectID,
                DateCreated = DateTime.Now,
            };
			database.File.Add(file);
			database.SaveChanges();
		}

        public void createDefaultFile(int projectID)
        {
            LanguageType projectType = (from x in database.Project
                                        where x.ID == projectID
                                        select x.LanguageType).First();
            File file = new File
            {
                Name = projectType.DefaultName,
                LanguageTypeID = projectType.ID,
                ProjectID = projectID,
                DateCreated = DateTime.Now
            };
            database.File.Add(file);
            database.SaveChanges();
        }

        public List<FileViewModel> getFileViewModelsForProject(int projectID)
        {
            List<FileViewModel> fileViewModels = new List<FileViewModel>();
            var files = getFilesForProject(projectID);

            foreach(var file in files)
            {
                fileViewModels.Add(convertFileToViewModel(file));
            }

            return fileViewModels;
        }

        public FileViewModel getFileViewModel(int fileID)
        {
            var file = (from x in database.File
                        where x.ID == fileID
                        select x).SingleOrDefault();
            var fileViewModel = convertFileToViewModel(file);
            return fileViewModel;
        }

        public List<File> getFilesForProject(int projectID)
        {
            var files = (from x in database.File
                         where x.ProjectID == projectID
                         select x).ToList();
            return files;
        }

        public FileViewModel convertFileToViewModel(File file)
        {
            FileViewModel model = new FileViewModel
            {
                ID = file.ID,
                Name = file.Name,
                Location = file.Location,
                LanguageTypeID = file.LanguageTypeID
            };
            return model;
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
            int userID = getUserIDByName(userName);

            var project = (from x in database.UserProject
                           where x.UserID == userID
                           && x.Project.Name == projectName
                           && x.Project.OwnerID == userID
                           select x.Project).SingleOrDefault();

            if (project != null)
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
                FileCount = 0,
                IsPublic = model.IsPublic,
                DateCreated = DateTime.Now,
                LanguageTypeID = model.LanguageTypeID
            };
            database.Project.Add(project);
            database.SaveChanges();

            int projectID = getProjectID(ownerID, model.Name);
            createDefaultFile(projectID);
        }

        public int getProjectID(int ownerID, string name)
        {
            int id = (from x in database.Project
                      where x.Name == name
                      && x.OwnerID == ownerID
                      select x.ID).First();
            return id;
        }
        /// <summary>
        /// Function which returns a MyProjectsViewModel containing both a
        /// list of project the user owns and list of projects he is collaborating on.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public MyProjectsViewModel getMyProjectsByName(string userName)
        {
            int userID = getUserIDByName(userName);

            var ownedProjects = getOwnedProjects(userID);
            List<ProjectViewModel> ownedProjectsViewModels = new List<ProjectViewModel>();
            foreach (var project in ownedProjects)
            {
                var projectVieModel = convertProjectToViewModel(project);
                ownedProjectsViewModels.Add(projectVieModel);
            }

            var collaborationProjects = getCollaborationProjects(userID);
            List<ProjectViewModel> collaboartionProjectsViewModel = new List<ProjectViewModel>();
            foreach (var project in collaborationProjects)
            {
                var projectVieModel = convertProjectToViewModel(project);
                collaboartionProjectsViewModel.Add(projectVieModel);
            }

            MyProjectsViewModel model = new MyProjectsViewModel
            {
                MyProjects = ownedProjectsViewModels,
                CollaborationProjects = collaboartionProjectsViewModel
            };

            return model;
        }

        public PublicProjectsViewModel getPublicProjects()
        {
            List<ProjectViewModel> publicProjects = new List<ProjectViewModel>();
            var result = (from p in database.Project
                          where p.IsPublic == true
                          select p).ToList();

            foreach (var project in result)
            {
                var projectViewModel = convertProjectToViewModel(project);
                publicProjects.Add(projectViewModel);
            }

            PublicProjectsViewModel model = new PublicProjectsViewModel
            {
                PublicProjects = publicProjects
            };

            return model;
        }

        public ProjectViewModel getProjectViewModelByID(int id)
        {
            var project = (from p in database.Project
                           where p.ID == id
                           select p).SingleOrDefault();
            if(project == null)
            {
                //Project does not exist!
                throw new Exception();
            }

            var projectViewModel = convertProjectToViewModel(project);
            projectViewModel.Files = getFileViewModelsForProject(id);


            return projectViewModel;
        }
        #endregion

        #region Private ProjectService
        /// <summary>
        /// Function which returns a list of every project a user owns.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Function which returns a list of every project a user is
        /// collaborating on, excluding projects he owns.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
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
                FileCount = project.FileCount,
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

		public IndexViewModel getUserAccountByUserName(string username)
		{
			User user = (from x in database.User
						 where x.Name == username
						 select x).SingleOrDefault();
			if (user == null)
			{
				throw new UserNotFoundException();
			}
			return new IndexViewModel
			{
				UserID = user.ID,
				Name = user.Name,
				Email = user.Email,
				Gender = user.Gender,
				UserTypeID = user.UserTypeID
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

        public SelectList getLanguageTypes()
        {
            return new SelectList(database.LanguageType, "ID", "Name");
        }

        public SelectList getLanguageTypes(int typeID)
        {
            return new SelectList(database.LanguageType, "ID", "Name", typeID);
        }
        #endregion
    }
}