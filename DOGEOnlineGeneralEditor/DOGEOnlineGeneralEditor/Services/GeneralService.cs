using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Models.ViewModels;
using DOGEOnlineGeneralEditor.Utilities;
using System.Web.Mvc;
using System.Data.Entity;

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
        public File GetFileById(int id)
        {
            var result = (from x in database.File
                          where x.ID == id
                          select x).SingleOrDefault();
            return result;
        }
        
        public int GetFileProjectID(int id)
        {
            int result = (from f in database.File
                          where f.ID == id
                          select f.ProjectID).SingleOrDefault();
            return result;
        }

        /// <summary>
        /// Function that returns true if the project with the given projectID contains a file
        /// whose filename is fileName.
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="fileName"></param>
        /// <returns>bool</returns>
        public bool FileExists(int projectID, string fileName)
        {
            File file = (from x in database.File
                         where x.Name == fileName
                         && x.Project.ID == projectID
                         select x).FirstOrDefault();
            if (file != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that returns true if the fileID is infact a valid file in the database.
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public bool FileExists(int fileID)
        {
            File file = database.File.Find(fileID);

            if(file != null)
            {
                return true;
            }
            return false;
        }

		public void AddFileToDatabase(CreateFileViewModel model)
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

        public void AddFileToDatabase(CreateFileFromFileViewModel model)
        {
            File file = new File
            {
                Name = model.PostedFile.FileName,
                LanguageTypeID = model.LanguageTypeID,
                ProjectID = model.ProjectID,
                Data = model.Data,
                DateCreated = DateTime.Now,
            };
            database.File.Add(file);
            database.SaveChanges();
        }

        public void CreateDefaultFile(int projectID)
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

        public void RemoveFile(int fileID)
        {
            File file = database.File.Find(fileID);
            database.File.Remove(file);
            database.SaveChanges();
        }

        public List<FileViewModel> GetFileViewModelsForProject(int projectID)
        {
            List<FileViewModel> fileViewModels = new List<FileViewModel>();
            var files = GetFilesForProject(projectID);

            foreach(var file in files)
            {
                fileViewModels.Add(ConvertFileToViewModel(file));
            }

            return fileViewModels;
        }

        public FileViewModel GetFileViewModel(int fileID)
        {
            var file = (from x in database.File
                        where x.ID == fileID
                        select x).SingleOrDefault();
            if(file == null)
            {
                throw new FileNotFoundException();
            }
            var fileViewModel = ConvertFileToViewModel(file);
            return fileViewModel;
        }

        public EditorViewModel GetEditorViewModel(string userName, int fileID)
        {
            int userID = GetUserIDByName(userName);
            var file = (from x in database.File
                        where x.ID == fileID
                        select x).SingleOrDefault();
            if(file == null)
            {
                throw new FileNotFoundException();
            }
            var editorViewModel = ConvertFileToEditorViewModel(userID, file);
            return editorViewModel;
        }

        public List<File> GetFilesForProject(int projectID)
        {
            var files = (from x in database.File
                         where x.ProjectID == projectID
                         select x).ToList();
            return files;
        }

        public FileViewModel ConvertFileToViewModel(File file)
        {
            FileViewModel model = new FileViewModel
            {
                ProjectID = file.ProjectID,
                ID = file.ID,
                Name = file.Name,
                Data = file.Data,
                LanguageTypeID = file.LanguageTypeID
            };
            return model;
        }

        public EditorViewModel ConvertFileToEditorViewModel(int userID, File file)
        {
            string aceTheme = GetUserTheme(userID);
            EditorViewModel model = new EditorViewModel
            {
                ProjectID = file.ProjectID,
                ID = file.ID,
                Name = file.Name,
                Data = file.Data,
                LanguageTypeID = file.LanguageTypeID,
                UserThemeID = aceTheme,
                LanguageTypes = GetLanguageTypeList(file.LanguageTypeID),
            };
            return model;
        }

        /// <summary>
        /// Function which saves an updated file to the database.
        /// </summary>
        /// <param name="model"></param>
        public void SaveFile(EditorViewModel model)
        {
            File file = database.File.Find(model.ID);
            file.Name = model.Name;
            file.Data = model.Data;
            file.LanguageTypeID = model.LanguageTypeID;
            database.Entry(file).State = EntityState.Modified;
            database.SaveChanges();
        }
        #endregion

        #region ProjectService
        /// <summary>
        /// Function that returns true if a user with the given Id has access to a project
        /// whose name is projectName
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public bool ProjectExists(string userName, string projectName)
        {
            int userID = GetUserIDByName(userName);

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
        
        public void AddProjectToDatabase(ProjectViewModel model)
        {
            int ownerID = GetUserIDByName(model.Owner);
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

            int projectID = GetProjectID(ownerID, model.Name);
            CreateDefaultFile(projectID);
        }

        public int GetProjectID(int ownerID, string name)
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
        public MyProjectsViewModel GetMyProjectsByName(string userName)
        {
            int userID = GetUserIDByName(userName);

            var ownedProjects = GetOwnedProjects(userID);
            List<ProjectViewModel> ownedProjectsViewModels = new List<ProjectViewModel>();
            foreach (var project in ownedProjects)
            {
                var projectVieModel = ConvertProjectToViewModel(project);
                ownedProjectsViewModels.Add(projectVieModel);
            }

            var collaborationProjects = GetCollaborationProjects(userID);
            List<ProjectViewModel> collaboartionProjectsViewModel = new List<ProjectViewModel>();
            foreach (var project in collaborationProjects)
            {
                var projectVieModel = ConvertProjectToViewModel(project);
                collaboartionProjectsViewModel.Add(projectVieModel);
            }

            MyProjectsViewModel model = new MyProjectsViewModel
            {
                MyProjects = ownedProjectsViewModels,
                CollaborationProjects = collaboartionProjectsViewModel
            };

            return model;
        }

        public PublicProjectsViewModel GetPublicProjects()
        {
            List<ProjectViewModel> publicProjects = new List<ProjectViewModel>();
            var result = (from p in database.Project
                          where p.IsPublic == true
                          select p).ToList();

            foreach (var project in result)
            {
                var projectViewModel = ConvertProjectToViewModel(project);
                publicProjects.Add(projectViewModel);
            }

            PublicProjectsViewModel model = new PublicProjectsViewModel
            {
                PublicProjects = publicProjects
            };

            return model;
        }

        public ProjectViewModel GetProjectViewModelByID(int id)
        {
            var project = (from p in database.Project
                           where p.ID == id
                           select p).SingleOrDefault();
            if(project == null)
            {
                //Project does not exist!
                throw new Exception();
            }

            var projectViewModel = ConvertProjectToViewModel(project);
            projectViewModel.Files = GetFileViewModelsForProject(id);


            return projectViewModel;
        }

        public bool RemoveProject(int projectID)
        {
            Project project = database.Project.Find(projectID);
            foreach(File file in project.Files.ToList())
            {
                database.File.Remove(file);
            }
            foreach(UserProject userProject in project.UserProjects.ToList())
            {
                database.UserProject.Remove(userProject);
            }
            database.Project.Remove(project);
            database.SaveChanges();
            return true;
        }

        public void EditProject(ProjectViewModel projectViewModel)
        {
            Project project = database.Project.Find(projectViewModel.ProjectID);
            project.Name = projectViewModel.Name;
            project.IsPublic = projectViewModel.IsPublic;
            project.LanguageTypeID = projectViewModel.LanguageTypeID;
            database.Entry(project).State = EntityState.Modified;
            database.SaveChanges();
        }


        #region Private ProjectService
        /// <summary>
        /// Function which returns a list of every project a user owns.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private List<Project> GetOwnedProjects(int userID)
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
        private List<Project> GetCollaborationProjects(int userID)
        {
            var result = (from p in database.Project
                          join up in database.UserProject
                          on p.ID equals up.ProjectID
                          where up.UserID == userID
                          && p.OwnerID != userID
                          select p).ToList();
            return result;
        }
        private ProjectViewModel ConvertProjectToViewModel(Project project)
        {
            string userName = GetUserNameByID(project.OwnerID);
            ProjectViewModel model = new ProjectViewModel
            {
                ProjectID = project.ID,
                Name = project.Name,
                FileCount = project.FileCount,
                Owner = userName,
                DateCreated = project.DateCreated,
                IsPublic = project.IsPublic
            };

            return model;
        }
        private int GetProjectIDFromViewModel(ProjectViewModel model)
        {
            int userID = GetUserIDByName(model.Owner);
            var project = (from x in database.Project
                           where x.Name == model.Name
                           && x.OwnerID == userID
                           select x).SingleOrDefault();
            return project.ID;
        }

        #endregion
        #endregion

        #region UserService

        public bool HasAccess(string userName, int projectID)
        {
            int userID = GetUserIDByName(userName);
            var result = (from x in database.UserProject
                          where x.UserID == userID
                          && x.ProjectID == projectID
                          select x).SingleOrDefault();

            if(result != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that returns a viewmodel of a user from the database. 
        /// If a user is not found it throws UserNotFoundException.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>UserViewModel</returns>
        public UserViewModel GetUserbyID(int userID)
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
        public UserViewModel GetUserByUserName(string username)
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

		public IndexViewModel GetUserAccountInfoByUserName(string username)
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
				Name = user.Name,
				Email = user.Email,
				Gender = user.Gender,
				UserTypeID = user.UserTypeID
			};
		}

        public string GetUserTheme(int userID)
        {
            string result = (from x in database.User
                             where x.ID == userID
                             select x.AceThemeID).SingleOrDefault();
            return result;
        }

        /// <summary>
        /// Function that adds a user to the database 
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns>bool</returns>
        public void CreateUser(RegisterViewModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                DateCreated = DateTime.Now,
                Gender = model.Gender,
                AceThemeID = "ace/theme/cobalt",
                UserTypeID = model.UserTypeID
            };

            database.User.Add(user);
            database.SaveChanges();
        }

		/// <summary>
		/// Function that adds a user to the database 
		/// </summary>
		/// <param name="applicationUser"></param>
		/// <returns>bool</returns>
		public void UpdateUser(IndexViewModel model)
		{
			int ID = GetUserIDByName(model.Name);
			User user = database.User.Find(ID);
			user.Email = model.Email;
			user.Gender = model.Gender;
			user.UserTypeID = model.UserTypeID;

			database.Entry(user).State = EntityState.Modified;
			database.SaveChanges();
		}

        public void UpdateUserTheme(int userID, string newTheme)
        {
            User user = database.User.Find(userID);
            user.AceThemeID = newTheme;
            database.Entry(user).State = EntityState.Modified;
            database.SaveChanges();
        }

        /// <summary>
        /// Function that returns true if a User with a given username exists in the database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>bool</returns>
        public bool UserExists(string name)
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

        public bool UserProjectExists(int userID, int projectID)
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
        public int GetUserIDByName(string userName)
        {
            int id = (from x in database.User
                      where x.Name == userName
                      select x.ID).SingleOrDefault();
            return id;
        }

        public string GetUserNameByID(int userID)
        {
            string name = (from x in database.User
                           where x.ID == userID
                           select x.Name).SingleOrDefault();
            return name;
        }
        public bool AddUserToProject(int userID, int projectID)
        {
            UserProject userProject = new UserProject
            {
                UserID = userID,
                ProjectID = projectID
            };
            if (UserProjectExists(userID, projectID))
            {
                // duplicateexception
                return false;
            }
            database.UserProject.Add(userProject);
            database.SaveChanges();
            return true;
        }
        public void AddUserToProject(string userName, ProjectViewModel project)
        {
            int userID = GetUserIDByName(userName);
            int projectID = GetProjectIDFromViewModel(project);
            UserProject userProject = new UserProject
            {
                UserID = userID,
                ProjectID = projectID
            };
            if(UserProjectExists(userID, projectID))
            {
                // duplicateexception
            }
            database.UserProject.Add(userProject);
            database.SaveChanges();
        }
        public bool RemoveUserProject (int userID, int projectID)
        {
            var userProject = (from up in database.UserProject
                               where up.UserID == userID
                               && up.ProjectID == projectID
                               select up).SingleOrDefault();
            if(userProject == null)
            {
                return false;
            }
            database.UserProject.Remove(userProject);
            database.SaveChanges();
            return true;
        }
        public UserCollabViewModel GetCollaboratorViewModel(string userName, int projectID)
        {
            int userID = GetUserIDByName(userName);
            var allUsers = (from up in database.UserProject
                            where up.UserID != userID
                            select up.User);

            var Collaborators = (from u in database.User
                                 join up in database.UserProject
                                 on u.ID equals up.UserID
                                 where up.ProjectID == projectID
                                 && u.ID != userID
                                 select u).Distinct();

            List <UserViewModel> CollaboratorList = new List<UserViewModel>();
            foreach (User user in Collaborators)
            {
                UserViewModel viewModel = new UserViewModel { UserID = user.ID, UserName = user.Name };
                CollaboratorList.Add(viewModel);
            }

            var notCollaborators = allUsers.Except(Collaborators);

            List<UserViewModel> notCollaboratorList = new List<UserViewModel>();
            foreach (User user in notCollaborators)
            {
                UserViewModel viewModel = new UserViewModel { UserID = user.ID, UserName = user.Name };
                notCollaboratorList.Add(viewModel);
            }
            UserCollabViewModel userCollabViewModel = new UserCollabViewModel
            {
                ProjectID = projectID,
                Collaborators = CollaboratorList,
                NotCollaborators = notCollaboratorList
            };
            return userCollabViewModel;

        }
        #endregion


        #region HelperFunctions
        // A List of functions needed to populate dropdown lists within the website
        // some take in a parameter to assign the default selected item while others don't.
        public SelectList GetUserTypes()
        {
            return new SelectList(database.UserType, "ID", "Name");
        }
        public SelectList GetUserTypes(int typeID)
        {
            return new SelectList(database.UserType, "ID", "Name", typeID);
        }
        public SelectList GetLanguageTypes()
        {
            return new SelectList(database.LanguageType, "ID", "Name");
        }
        public SelectList GetLanguageTypes(int typeID)
        {
            return new SelectList(database.LanguageType, "ID", "Name", typeID);
        }
        public List<LanguageType> GetLanguageTypeList(int typeID)
        {
            List<LanguageType> list = new List<LanguageType>();
            var result = (from x in database.LanguageType
                          select x).ToList();
            result.ForEach(c => list.Add(c));
            return result;
        }
        public SelectList GetAceThemes(string themeID)
        {
            return new SelectList(database.AceTheme, "ID", "Theme", themeID);
        }

        #endregion
    }
}