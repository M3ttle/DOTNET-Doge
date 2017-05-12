using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Services;
using DOGEOnlineGeneralEditor.Utilities;

namespace DOGEOnlineGeneralEditor.Tests.Services
{
    [TestClass]
    public class FileServiceTest
    {
        private GeneralService service;

        [TestInitialize]
        public void Initialize()
        {
            var mockDB = new MockDataContext();

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
            mockDB.Project.Add(p2);

            var f1 = new File
            {
                ID = 1,
                Name = "index.css",
                Data = "TestData1",
                DateCreated = DateTime.Now,
                ProjectID = 2,
                LanguageTypeID = 1,
                Project = p2
            };
            mockDB.File.Add(f1);
            var f2 = new File
            {
                ID = 2,
                Name = "main.cpp",
                Data = "TestData2",
                DateCreated = DateTime.Now,
                ProjectID = 1,
                LanguageTypeID = 2,
                Project = p1
            };
            mockDB.File.Add(f2);
            var f3 = new File
            {
                ID = 3,
                Name = "node.cpp",
                Data = "TestData2",
                ProjectID = 1,
                LanguageTypeID = 2,
                Project = p1
            };
            mockDB.File.Add(f3);

            service = new GeneralService(mockDB);
        }

        [TestMethod]
        public void TestGetProjectIDOfFile()
        {
            const int fileID = 1;
            const int projectID = 2;

            var resultID = service.GetProjectIDOfFile(fileID);

            Assert.AreEqual(projectID, resultID);
        }

        [TestMethod]
        public void TestFileExists()
        {
            const int fileID = 20;
            const int projectID = 2;
            const string fileName = "index.css";

            bool result = service.FileExists(projectID, fileName, fileID);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestFileDoesNotExist()
        {
            const string fileName = "main.cpp";
            const int projectID = 2;

            var result = service.FileExists(projectID, fileName);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestGetFilesForProject()
        {
            const int projectID = 1;
            const int expectedResult = 2;

            var result = service.GetFilesForProject(projectID);

            Assert.AreEqual(expectedResult, result.Count);
            foreach (var item in result)
            {
                Assert.AreNotEqual(item.Name, "index.css");
            }
        }

        [TestMethod]
        public void TestGetFilesForEmptyProject()
        {
            const int projectID = 3;
            const int expectedResult = 0;

            var result = service.GetFilesForProject(projectID);

            Assert.AreEqual(expectedResult, result.Count);
        }

        [TestMethod]
        public void TestGetEditorViewModel()
        {
            const int fileID = 1;
            const string userName = "Matti";
            const string expectedResult = "TestData1";

            var result = service.GetEditorViewModel(userName, fileID);

            Assert.AreEqual(expectedResult, result.Data);
        }

        [TestMethod]
        [ExpectedException(typeof(CustomFileNotFoundException))]
        public void TestGetEditorViewModelForInvalidFile()
        {
            const int fileID = 40;
            const string invalidUserName = "Irrelevant Name";

            var result = service.GetEditorViewModel(invalidUserName, fileID);

            Assert.Fail();
        }
    }
}
