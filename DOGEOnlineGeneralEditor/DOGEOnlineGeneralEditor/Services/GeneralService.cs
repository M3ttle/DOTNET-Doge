using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Models.ViewModels;
using DOGEOnlineGeneralEditor.Utilities;

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
        #endregion
        /// <summary>
        /// Function that returns true if a user with the given Id has access to a project
        /// whose name is projectName
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        #region ProjectService
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

        public UserViewModel getUserbyUsername(string username)
        {
            User user = (from x in database.User
                         where x.Name.Contains(username)
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
        bool AddUser(ApplicationUser applicationUser)
        {
            User user = new User
            {
                Name = applicationUser.UserName,
                DateCreated = DateTime.Now,
                //ná í UserType og Gender
            };
            database.User.Add(user);

            if(database.SaveChanges() == 1)
            {
                return true;
            }
            return false;
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
        #endregion
    }
}