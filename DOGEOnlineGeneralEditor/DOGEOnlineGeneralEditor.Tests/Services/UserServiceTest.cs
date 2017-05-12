using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOGEOnlineGeneralEditor.Services;
using DOGEOnlineGeneralEditor.Models.POCO;

namespace DOGEOnlineGeneralEditor.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private GeneralService service;

        [TestInitialize]
        public void Initialize()
        {
            var mockDB = new MockDataContext();

            var u1 = new User
            {
                ID = 1,
                Name = "Matti",
                AceThemeID = "ace/theme/chrome"
            };
            mockDB.User.Add(u1);
            var u2 = new User
            {
                ID = 2,
                Name = "Nonni",
                AceThemeID = "ace/theme/monokai"
            };
            mockDB.User.Add(u2);
            var p1 = new Project
            {
                ID = 1,
                Name = "MyFirstProject",
                OwnerID = 1,
                IsPublic = true,
                LanguageTypeID = 1
            };
            mockDB.Project.Add(p1);
            var p2 = new Project
            {
                ID = 2,
                Name = "MySecondProject",
                OwnerID = 1,
                IsPublic = false,
                LanguageTypeID = 1
            };
            mockDB.Project.Add(p2);
            var p3 = new Project
            {
                ID = 3,
                Name = "MyThirdProject",
                OwnerID = 2,
                IsPublic = false,
                LanguageTypeID = 1
            };
            mockDB.Project.Add(p3);

            var up1 = new UserProject
            {
                ID = 1,
                UserID = 1,
                ProjectID = 1,
                User = u1,
                Project = p1
            };
            mockDB.UserProject.Add(up1);
            var up2 = new UserProject
            {
                ID = 2,
                UserID = 1,
                ProjectID = 2,
                User = u1,
                Project = p2
            };
            mockDB.UserProject.Add(up2);
            var up3 = new UserProject
            {
                ID = 3,
                UserID = 2,
                ProjectID = 3,
                User = u2,
                Project = p3
            };
            mockDB.UserProject.Add(up3);
            var up4 = new UserProject
            {
                ID = 4,
                UserID = 2,
                ProjectID = 2,
                User = u2,
                Project = p2
            };
            mockDB.UserProject.Add(up4);


            service = new GeneralService(mockDB);
        }

        [TestMethod]
        public void TestHasAccess()
        {
            const string userName = "Matti";
            const int projectID = 1;

            var result = service.HasAccess(userName, projectID);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDoesNotHaveAccess()
        {
            const string userName = "Matti";
            const int projectID = 3;

            var result = service.HasAccess(userName, projectID);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestGetUserTheme()
        {
            const int userID = 2;
            const string expectedResult = "ace/theme/monokai";

            var result = service.GetUserTheme(userID);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestGetUserID()
        {
            const string userName = "Nonni";
            const int expectedResult = 2;

            var result = service.GetUserID(userName);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestGetUserName()
        {
            const int userID = 2;
            const string expectedResult = "Nonni";

            var result = service.GetUserName(userID);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestUserProjectExists()
        {
            const int userID = 1;
            const int projectID = 2;

            var result = service.UserProjectExists(userID, projectID);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestUserProjectDoesNotExist()
        {
            const int userID = 2;
            const int projectID = 1;

            var result = service.UserProjectExists(userID, projectID);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestAddUserToProjectAlreadyIn()
        {
            const int userID = 2;
            const int projectID = 3;

            var result = service.AddUserToProject(userID, projectID);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestRemoveUserFromProjectHeIsNotIn()
        {
            const int userID = 2;
            const int projectID = 1;

            var result = service.RemoveUserProject(userID, projectID);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestGetCollaboratorViewModel()
        {
            const string userName = "Matti";
            const int projectID = 2;
            const int expectedResult = 1;

            var result = service.GetCollaboratorViewModel(userName, projectID);

            Assert.AreEqual(expectedResult, result.Collaborators.Count);
        }

        [TestMethod]
        public void TestGetCollaboratorViewModelForNoCollaborators()
        {
            const string userName = "Matti";
            const int projectID = 1;
            const int expectedResult = 0;

            var result = service.GetCollaboratorViewModel(userName, projectID);

            Assert.AreEqual(expectedResult, result.Collaborators.Count);
        }
    }
}
