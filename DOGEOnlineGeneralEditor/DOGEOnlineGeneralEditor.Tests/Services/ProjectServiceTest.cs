using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOGEOnlineGeneralEditor.Services;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Utilities;

namespace DOGEOnlineGeneralEditor.Tests.Services
{
    [TestClass]
    public class ProjectServiceTest
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
            };
            mockDB.User.Add(u1);
            var u2 = new User
            {
                ID = 2,
                Name = "Nonni",
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
                IsPublic = true,
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
                UserID = 1,
                ProjectID = 3,
                User = u1,
                Project = p3
            };
            mockDB.UserProject.Add(up4);

            service = new GeneralService(mockDB);
        }

        [TestMethod]
        public void TestProjectExists()
        {
            const string userName = "Matti";
            const string projectName = "MyFirstProject";

            var result = service.ProjectExists(userName, projectName);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestGetProjectID()
        {
            const int userID = 2;
            const string projectName = "MyThirdProject";
            const int expectedResult = 3;

            var resultID = service.GetProjectID(userID, projectName);

            Assert.AreEqual(expectedResult, resultID);
        }

        [TestMethod]
        public void TestGetInvalidProjectID()
        {
            const int userID = 2;
            const string projectName = "InvalidProject";
            const int expectedResult = 0;

            var resultID = service.GetProjectID(userID, projectName);

            Assert.AreEqual(expectedResult, resultID);
        }

        [TestMethod]
        public void TestGetMyProjects()
        {
            const string userName = "Matti";
            const int expectedResult1 = 2;
            const int expectedResult2 = 1;
            const string nameOfThirdProject = "MyThirdProject";

            var result = service.GetMyProjects(userName);

            Assert.AreEqual(expectedResult1, result.MyProjects.Count);
            foreach(var item in result.MyProjects)
            {
                Assert.AreNotEqual(nameOfThirdProject, item.Name);
            }
            Assert.AreEqual(expectedResult2, result.CollaborationProjects.Count);
            foreach(var item in result.CollaborationProjects)
            {
                Assert.AreEqual(nameOfThirdProject, item.Name);
            }
        }

        [TestMethod]
        public void TestGetMyProjectsForCthulhu()
        {
            const string userName = "Cthulhu";
            const int expectedResult = 0;

            var result = service.GetMyProjects(userName);

            Assert.AreEqual(expectedResult, result.MyProjects.Count);
            Assert.AreEqual(expectedResult, result.CollaborationProjects.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(CustomProjectNotFoundException))]
        public void TestGetInvalidProjectViewModel()
        {
            const int invalidProjectID = 42;

            var result = service.GetProjectViewModel(invalidProjectID);

            Assert.Fail();
        }

        [TestMethod]
        public void TestGetPublicProjects()
        {
            const int expectedResult = 2;
            const string nameOfNotPublicProject = "MySecondProject";

            var result = service.GetPublicProjects();

            Assert.AreEqual(expectedResult, result.PublicProjects.Count);

            foreach(var item in result.PublicProjects)
            {
                Assert.AreNotEqual(nameOfNotPublicProject, item.Name);
            }
        }
    }
}
