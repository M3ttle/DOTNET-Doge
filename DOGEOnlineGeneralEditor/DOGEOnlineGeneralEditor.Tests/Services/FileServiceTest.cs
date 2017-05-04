using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Services;

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
            var f1 = new File
            {
                ID = 1,
                Name = "test",
                Location = "someLoc",
                DateCreated = DateTime.Now,
                ProjectID = 1,
                LanguageTypeID = 1
            };
            mockDB.File.Add(f1);


            service = new GeneralService(mockDB);
        }

        [TestMethod]
        public void TestFile()
        {
            const int id = 1;

            var file = service.getFileById(id);

            Assert.AreEqual(id, file.ID);
        }
    }
}
