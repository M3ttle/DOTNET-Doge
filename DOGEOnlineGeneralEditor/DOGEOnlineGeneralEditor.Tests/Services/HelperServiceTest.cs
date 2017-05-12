using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DOGEOnlineGeneralEditor.Services;
using DOGEOnlineGeneralEditor.Models.POCO;
using System.Linq;

namespace DOGEOnlineGeneralEditor.Tests.Services
{
    [TestClass]
    public class HelperServiceTest
    {
        private GeneralService service;

        [TestInitialize]
        public void Initialize()
        {
            var mockDB = new MockDataContext();

            var lt1 = new LanguageType
            {
                ID = 1,
                Name = "CSS",
                DefaultName = "index.css",
                AceMode = "ace/mode/CSS",
            };
            mockDB.LanguageType.Add(lt1);
            var lt2 = new LanguageType
            {
                ID = 2,
                Name = "C and C++",
                DefaultName = "main.cpp",
                AceMode = "ace/mode/c_cpp",
            };
            mockDB.LanguageType.Add(lt2);
            var lt3 = new LanguageType
            {
                ID = 3,
                Name = "C#",
                DefaultName = "main.cs",
                AceMode = "ace/mode/csharp",
            };
            mockDB.LanguageType.Add(lt3);

            service = new GeneralService(mockDB);
        }

        [TestMethod]
        public void TestGetLanguageTypes()
        {
            const int expectedResult = 3;

            var result = service.GetLanguageTypes();

            Assert.AreEqual(expectedResult, result.Count());
        }

        [TestMethod]
        public void TestGetLanguageTypesWithSelect()
        {
            const int typeID = 1;

            var result = service.GetLanguageTypes(typeID);

            Assert.AreEqual(typeID, result.SelectedValue);
        }
    }
}
