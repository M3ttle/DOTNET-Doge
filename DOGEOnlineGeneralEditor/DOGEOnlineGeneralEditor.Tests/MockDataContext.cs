using System.Data.Entity;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Models;
using System;
using System.Data.Entity.Infrastructure;

namespace DOGEOnlineGeneralEditor.Tests
{
	class MockDataContext : IAppDataContext
	{
		/// <summary>
		/// Sets up the fake database.
		/// </summary>
		public MockDataContext()
		{
			// We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
			Project = new InMemoryDbSet<Project>();
            UserProject = new InMemoryDbSet<UserProject>();
            User = new InMemoryDbSet<User>();
            LanguageType = new InMemoryDbSet<LanguageType>();
            File = new InMemoryDbSet<File>();
            UserType = new InMemoryDbSet<UserType>();
		}

        public IDbSet<Project> Project { get; set; }
        public IDbSet<UserProject> UserProject { get; set; }
        public IDbSet<User> User { get; set; }
        public IDbSet<LanguageType> LanguageType { get; set; }
        public IDbSet<File> File { get; set; }
        public IDbSet<UserType> UserType { get; set; }
        // TODO: bætið við fleiri færslum hér
        // eftir því sem þeim fjölgar í AppDataContext klasanum ykkar!

        public int SaveChanges()
		{
			// Pretend that each entity gets a database id when we hit save.
			int changes = 0;

			return changes;
		}

		public void Dispose()
		{
			// Do nothing!
		}

        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
