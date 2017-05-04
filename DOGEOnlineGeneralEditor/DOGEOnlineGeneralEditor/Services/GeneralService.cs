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
        #endregion

        #region ProjectService
        #endregion
        #region UserService
        /// <summary>
        /// Function that returns a viewmodel of a user from the database. 
        /// If a user is not found it throws UserNotFoundException.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>UserViewModel</returns>
        public UserViewModel getUser(int userID)
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
        /// Function that adds a user to the database 
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <returns></returns>
        bool AddUser(ApplicationUser applicationUser)
        {
            User user = new User
            {
                Name = applicationUser.UserName,
                DateCreated = DateTime.Now,
                //ná í UserType og Gender
            };
            database.User.Add(user);
            database.SaveChanges();
            return false;
        }
        #endregion
    }
}