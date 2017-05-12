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
        /// <summary>
        /// Returns the ID of the project that contains the given file ID.
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns>int</returns>
        public int GetProjectIDOfFile(int fileID)
        {
            int projectID = (from f in database.File
                             where f.ID == fileID
                             select f.ProjectID).SingleOrDefault();
            return projectID;
        }
        /// <summary>
        /// Function that returns true if the project with the given project ID contains a file
        /// whose name is the given file name.
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="fileName"></param>
        /// <returns>bool</returns>
        public bool FileExists(int projectID, string fileName)
        {
            var file = (from f in database.File
                        where f.Name == fileName
                        && f.Project.ID == projectID
                        select f).FirstOrDefault();
            if (file != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that returns true if a project contains a file with the given name
        /// and the file is not the existing file.
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="fileName"></param>
        /// <param name="fileID"></param>
        /// <returns>bool</returns>
        public bool FileExists(int projectID, string fileName, int fileID)
        {
            var file = (from f in database.File
                        where f.Name == fileName
                        && f.Project.ID == projectID
                        && f.ID != fileID
                        select f).SingleOrDefault();
            if (file != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that returns true if a file with the given ID exists in the database.
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns>bool</returns>
        public bool FileExists(int fileID)
        {
            var file = database.File.Find(fileID);
            if (file != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that creates a file and adds it to the database.
        /// </summary>
        /// <param name="model"></param>
		public void CreateFile(CreateFileViewModel model)
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
        /// <summary>
        /// Function that creates a file from an uploaded file and adds it to the database.
        /// </summary>
        /// <param name="model"></param>
        public void CreateFile(CreateFileFromFileViewModel model)
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
        /// <summary>
        /// Function that checks whether the MIME type of a file being uploaded is supported
        /// or not.
        /// </summary>
        /// <param name="MIMEType"></param>
        /// <returns></returns>
        public bool MIMETypeIsValid(string MIMEType)
        {
            List<string> typeList = new List<string>
            {
                "text/html",
                "text/plain",
                "text/css",
                "application/javascript",
                "application/json",
                "text/javascript"
            };

            foreach (var type in typeList)
            {
                if (type == MIMEType)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Function that creates the default file for a new project, whose file name 
        /// depends on the language type of the project.
        /// </summary>
        /// <param name="projectID"></param>
        public void CreateDefaultFile(int projectID)
        {
            var languageType = (from p in database.Project
                                where p.ID == projectID
                                select p.LanguageType).First();
            File file = new File
            {
                Name = languageType.DefaultName,
                LanguageTypeID = languageType.ID,
                ProjectID = projectID,
                DateCreated = DateTime.Now
            };

            database.File.Add(file);
            database.SaveChanges();
        }
        /// <summary>
        /// Function that removes the file with the given ID from the database.
        /// </summary>
        /// <param name="fileID"></param>
        public void RemoveFile(int fileID)
        {
            var file = database.File.Find(fileID);

            database.File.Remove(file);
            database.SaveChanges();
        }
        /// <summary>
        /// Function that returns a List of FileViewModels belonging to a given project.
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>List<FileViewModel></returns>
        public List<FileViewModel> GetFileViewModelsForProject(int projectID)
        {
            List<FileViewModel> fileViewModels = new List<FileViewModel>();
            var files = GetFilesForProject(projectID);

            foreach (var file in files)
            {
                fileViewModels.Add(ConvertFileToViewModel(file));
            }

            return fileViewModels;
        }
        /// <summary>
        /// Function that returns a EditorViewModel for the given file ID containing
        /// the users Ace Editor Theme.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public EditorViewModel GetEditorViewModel(string userName, int fileID)
        {
            int userID = GetUserID(userName);

            var file = (from f in database.File
                        where f.ID == fileID
                        select f).SingleOrDefault();
            if (file == null)
            {
                throw new CustomFileNotFoundException();
            }

            var editorViewModel = ConvertFileToEditorViewModel(userID, file);
            return editorViewModel;
        }
        /// <summary>
        /// Returns a List of Files that a Project contains
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>List<File></returns>
        public List<File> GetFilesForProject(int projectID)
        {
            var files = (from f in database.File
                         where f.ProjectID == projectID
                         select f).ToList();
            return files;
        }
        /// <summary>
        /// Function which saves an updated file to the database.
        /// </summary>
        /// <param name="model"></param>
        public void SaveFile(EditorViewModel model)
        {
            var file = database.File.Find(model.ID);

            file.Name = model.Name;
            file.Data = model.Data;
            file.LanguageTypeID = model.LanguageTypeID;

            database.Entry(file).State = EntityState.Modified;
            database.SaveChanges();
        }
        #region Private FileService
        /// <summary>
        /// Function that creates a EditorViewModel from a file.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="file"></param>
        /// <returns>EditorViewModel</returns>
        private EditorViewModel ConvertFileToEditorViewModel(int userID, File file)
        {
            string aceTheme = GetUserTheme(userID);

            return new EditorViewModel
            {
                ProjectID = file.ProjectID,
                ID = file.ID,
                Name = file.Name,
                Data = file.Data,
                LanguageTypeID = file.LanguageTypeID,
                UserThemeID = aceTheme,
                LanguageTypes = GetLanguageTypeList()
            };
        }
        /// <summary>
        /// Function that creates a FileViewModel from a file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>FileViewModel</returns>
        private FileViewModel ConvertFileToViewModel(File file)
        {
            return new FileViewModel
            {
                ProjectID = file.ProjectID,
                ID = file.ID,
                Name = file.Name,
                Data = file.Data,
                LanguageTypeID = file.LanguageTypeID
            };
        }
        #endregion
        #endregion

        #region ProjectService
        /// <summary>
        /// Function that returns true if a User is the owner of the Project
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="projectName"></param>
        /// <returns>bool</returns>
        public bool ProjectExists(string userName, string projectName)
        {
            int userID = GetUserID(userName);

            var project = (from up in database.UserProject
                           where up.UserID == userID
                           && up.Project.Name == projectName
                           && up.Project.OwnerID == userID
                           select up.Project).SingleOrDefault();
            if (project != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that adds a Project to the database.
        /// </summary>
        /// <param name="model"></param>
        public void CreateProject(ProjectViewModel model)
        {
            int ownerID = GetUserID(model.Owner);

            Project project = new Project
            {
                Name = model.Name,
                OwnerID = ownerID,
                IsPublic = model.IsPublic,
                DateCreated = DateTime.Now,
                LanguageTypeID = model.LanguageTypeID,
            };

            database.Project.Add(project);
            database.SaveChanges();

            int projectID = GetProjectID(ownerID, model.Name);
            CreateDefaultFile(projectID);
        }
        /// <summary>
        /// Function that returns the ID of a Project whose Name is name and is owned by 
        /// the User with the ID userID.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="name"></param>
        /// <returns>int</returns>
        public int GetProjectID(int userID, string name)
        {
            int projectID = (from p in database.Project
                             where p.Name == name
                             && p.OwnerID == userID
                             select p.ID).FirstOrDefault();
            return projectID;
        }
        /// <summary>
        /// Function which returns a MyProjectsViewModel containing both a
        /// List of project the user owns and List of projects he is collaborating on.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public MyProjectsViewModel GetMyProjects(string userName)
        {
            int userID = GetUserID(userName);

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

            return new MyProjectsViewModel
            {
                MyProjects = ownedProjectsViewModels,
                CollaborationProjects = collaboartionProjectsViewModel
            };
        }
        /// <summary>
        /// Function which returns a List of all public Projects.
        /// </summary>
        /// <returns></returns>
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

            PublicProjectsViewModel publicProjectsViewModel = new PublicProjectsViewModel
            {
                PublicProjects = publicProjects
            };

            return publicProjectsViewModel;
        }
        /// <summary>
        /// Function that return a project viewmodel with a List of all the files
        /// belonging to the given project.
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ProjectViewModel GetProjectViewModel(int projectID)
        {
            var project = (from p in database.Project
                           where p.ID == projectID
                           select p).SingleOrDefault();
            if (project == null)
            {
                throw new CustomProjectNotFoundException();
            }

            var projectViewModel = ConvertProjectToViewModel(project);
            projectViewModel.Files = GetFileViewModelsForProject(projectID);

            return projectViewModel;
        }
        /// <summary>
        /// Function that removes the project with the given ID from the database.
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool RemoveProject(int projectID)
        {
            var project = database.Project.Find(projectID);

            foreach (File file in project.Files.ToList())
            {
                database.File.Remove(file);
            }
            foreach (UserProject userProject in project.UserProjects.ToList())
            {
                database.UserProject.Remove(userProject);
            }

            database.Project.Remove(project);
            database.SaveChanges();
            return true;
        }
        /// <summary>
        /// Function that updates the general information of a project.
        /// </summary>
        /// <param name="model"></param>
        public void UpdateProject(ProjectViewModel model)
        {
            var project = database.Project.Find(model.ID);

            project.Name = model.Name;
            project.IsPublic = model.IsPublic;
            project.LanguageTypeID = model.LanguageTypeID;
            database.Entry(project).State = EntityState.Modified;
            database.SaveChanges();
        }

        #region Private ProjectService
        /// <summary>
        /// Function that returns a List of every project a user owns.
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
        /// Function that returns a list of every project a user is
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
        /// <summary>
        /// Function that converts a project model into a ProjectViewModel.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ProjectViewModel ConvertProjectToViewModel(Project model)
        {
            string userName = GetUserName(model.OwnerID);

            return new ProjectViewModel
            {
                ID = model.ID,
                Name = model.Name,
                Owner = userName,
                DateCreated = model.DateCreated,
                IsPublic = model.IsPublic
            };
        }
        #endregion
        #endregion

        #region UserService
        /// <summary>
        /// Function that returns whether a user has access to a given project or not.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool HasAccess(string userName, int projectID)
        {
            var isPublic = (from p in database.Project
                           where p.ID == projectID
                           select p.IsPublic).SingleOrDefault();
            if (isPublic)
            {
                return true;
            }

            int userID = GetUserID(userName);
            var result = (from up in database.UserProject
                          where up.UserID == userID
                          && up.ProjectID == projectID
                          select up).SingleOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that returns a IndexViewModel for a given users name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
		public IndexViewModel GetUserIndexViewModel(string userName)
        {
            var user = (from u in database.User
                        where u.Name == userName
                        select u).SingleOrDefault();

            return new IndexViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender,
                UserTypeID = user.UserTypeID
            };
        }
        /// <summary>
        /// Function that returns a given users Ace Editor theme.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetUserTheme(int userID)
        {
            string result = (from u in database.User
                             where u.ID == userID
                             select u.AceThemeID).SingleOrDefault();
            return result;
        }
        /// <summary>
        /// Function that creates and adds a user to the database.
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns>bool</returns>
        public void CreateUser(RegisterViewModel model)
        {
            User user = new User
            {
                Name = model.Name,
                Email = model.Email,
                DateCreated = DateTime.Now,
                Gender = model.Gender,
                AceThemeID = "ace/theme/monokai",
                UserTypeID = model.UserTypeID
            };

            database.User.Add(user);
            database.SaveChanges();
        }
        /// <summary>
        /// Function that updates a users general information in the database.
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns>bool</returns>
        public void UpdateUser(IndexViewModel model)
        {
            int ID = GetUserID(model.Name);
            var user = database.User.Find(ID);

            user.Email = model.Email;
            user.Gender = model.Gender;
            user.UserTypeID = model.UserTypeID;

            database.Entry(user).State = EntityState.Modified;
            database.SaveChanges();
        }
        /// <summary>
        /// Function that updates a users Ace Editor theme in the database.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="newTheme"></param>
        public void UpdateUserTheme(int userID, string newTheme)
        {
            var user = database.User.Find(userID);

            user.AceThemeID = newTheme;

            database.Entry(user).State = EntityState.Modified;
            database.SaveChanges();
        }
        /// <summary>
        /// Function that returns true if the user ID is associated with a given project ID.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool UserProjectExists(int userID, int projectID)
        {
            var userProject = (from up in database.UserProject
                               where up.UserID == userID
                               && up.ProjectID == projectID
                               select up).SingleOrDefault();
            if (userProject != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function that returns the user ID of a user with the given user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GetUserID(string userName)
        {
            int userID = (from u in database.User
                          where u.Name == userName
                          select u.ID).SingleOrDefault();
            return userID;
        }
        /// <summary>
        /// Function that returns the name of a given users ID.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetUserName(int userID)
        {
            string userName = (from u in database.User
                               where u.ID == userID
                               select u.Name).SingleOrDefault();
            return userName;
        }
        /// <summary>
        /// Function that adds an association between a user and a project.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool AddUserToProject(int userID, int projectID)
        {
            UserProject userProject = new UserProject
            {
                UserID = userID,
                ProjectID = projectID
            };
            if (UserProjectExists(userID, projectID))
            {
                return false;
            }

            database.UserProject.Add(userProject);
            database.SaveChanges();
            return true;
        }
        /// <summary>
        /// Function that adds an association between a user and a project.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void AddUserToProject(string userName, ProjectViewModel model)
        {
            int userID = GetUserID(userName);
            int ownerID = GetUserID(model.Owner);
            int projectID = GetProjectID(ownerID, model.Name);

            UserProject userProject = new UserProject
            {
                UserID = userID,
                ProjectID = projectID
            };

            database.UserProject.Add(userProject);
            database.SaveChanges();
        }
        /// <summary>
        /// Function that removes a connection between a user and a project.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool RemoveUserProject(int userID, int projectID)
        {
            var userProject = (from up in database.UserProject
                               where up.UserID == userID
                               && up.ProjectID == projectID
                               select up).SingleOrDefault();
            if (userProject == null)
            {
                return false;
            }

            database.UserProject.Remove(userProject);
            database.SaveChanges();
            return true;
        }
        /// <summary>
        /// Function that returns two lists, one of users which are collaborators of a given
        /// project and one of users which are not.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public UserCollaboratorViewModel GetCollaboratorViewModel(string userName, int projectID)
        {
            int userID = GetUserID(userName);

            var allUsers = (from up in database.UserProject
                            where up.UserID != userID
                            select up.User);

            var Collaborators = (from u in database.User
                                 join up in database.UserProject
                                 on u.ID equals up.UserID
                                 where up.ProjectID == projectID
                                 && u.ID != userID
                                 select u).Distinct();

            List<UserViewModel> CollaboratorList = new List<UserViewModel>();
            foreach (User user in Collaborators)
            {
                UserViewModel viewModel = new UserViewModel { ID = user.ID, Name = user.Name };
                CollaboratorList.Add(viewModel);
            }

            var notCollaborators = allUsers.Except(Collaborators);

            List<UserViewModel> notCollaboratorList = new List<UserViewModel>();
            foreach (User user in notCollaborators)
            {
                UserViewModel viewModel = new UserViewModel { ID = user.ID, Name = user.Name };
                notCollaboratorList.Add(viewModel);
            }
            UserCollaboratorViewModel userCollaboratorViewModel = new UserCollaboratorViewModel
            {
                ProjectID = projectID,
                Collaborators = CollaboratorList,
                NotCollaborators = notCollaboratorList
            };
            return userCollaboratorViewModel;
        }
        #endregion

        #region HelperFunctions
        // A List of functions needed to populate dropdown lists within the website
        // some take in a parameter to assign the default selected item while others don't.

        /// <summary>
        /// Function that returns a SelectList of all user types currently in the database.
        /// </summary>
        /// <returns></returns>
        public SelectList GetUserTypes()
        {
            return new SelectList(database.UserType, "ID", "Name");
        }
        /// <summary>
        /// Function that returns a SelectList of all user types currently in the database
        /// where the given type ID is selected.
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public SelectList GetUserTypes(int typeID)
        {
            return new SelectList(database.UserType, "ID", "Name", typeID);
        }
        /// <summary>
        /// Function that returns a SelectList of all language types currently in the database.
        /// </summary>
        /// <returns></returns>
        public SelectList GetLanguageTypes()
        {
            return new SelectList(database.LanguageType, "ID", "Name");
        }
        /// <summary>
        /// Function that returns a SelectList of all language types currently in the database
        /// where the given type ID is selected.
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public SelectList GetLanguageTypes(int typeID)
        {
            return new SelectList(database.LanguageType, "ID", "Name", typeID);
        }
        /// <summary>
        /// Function that returns a List of Languagetypes 
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public List<LanguageType> GetLanguageTypeList()
        {
            List<LanguageType> list = new List<LanguageType>();
            var result = (from lt in database.LanguageType
                          select lt).ToList();
            result.ForEach(c => list.Add(c));
            return result;
        }
        /// <summary>
        /// Function that returns a SelectList of all Ace Editor themes currently in the database
        /// where the given theme ID is selected.
        /// </summary>
        /// <param name="themeID"></param>
        /// <returns></returns>
        public SelectList GetAceThemes(string themeID)
        {
            return new SelectList(database.AceTheme, "ID", "Theme", themeID);
        }
        #endregion
    }
}