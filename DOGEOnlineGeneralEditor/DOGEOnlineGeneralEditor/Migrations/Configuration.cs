namespace DOGEOnlineGeneralEditor.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using DOGEOnlineGeneralEditor.Models.POCO;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<DOGEOnlineGeneralEditor.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DOGEOnlineGeneralEditor.Models.ApplicationDbContext";
        }

        protected override void Seed(DOGEOnlineGeneralEditor.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var languageTypes = new List<LanguageType>
            {
                new LanguageType{ Name = "Javascript", DefaultName = "index.js", AceMode = "ace/mode/javascript"},
                new LanguageType{ Name = "HTML", DefaultName = "index.html", AceMode = "ace/mode/html"},
                new LanguageType{ Name = "C++", DefaultName = "main.cpp", AceMode = "ace/mode/c_cpp"},
                new LanguageType{ Name = "CSS", DefaultName = "index.css", AceMode = "ace/mode/css"},
                new LanguageType{ Name = "C#", DefaultName = "main.cs", AceMode = "ace/mode/csharp"},
            };
            languageTypes.ForEach(s => context.LanguageType.AddOrUpdate(p => p.Name, s));
            /*
            foreach (LanguageType item in languageTypes)
            {
                context.LanguageType.AddOrUpdate(s => s.Name == item.Name);
            }*/
            context.SaveChanges();
            var userTypes = new List<UserType>
            {
                new UserType{ Name = "Student" },
                new UserType{ Name = "Teacher" },
                new UserType{ Name = "Programmer" },
                new UserType{ Name = "Other" },
            };
            userTypes.ForEach(s => context.UserType.AddOrUpdate(p => p.Name, s));
            /*
            foreach (UserType item in userTypes)
            {
                context.UserType.AddOrUpdate(s => s.Name == item.Name);
            }*/
            context.SaveChanges();

            var aceThemes = new List<AceTheme>
            {
                new AceTheme {ID = "ace/theme/ambiance", Theme = "Ambiance"},
                new AceTheme {ID = "ace/theme/chrome", Theme = "Chrome"},
                new AceTheme {ID = "ace/theme/cobalt", Theme = "Cobalt"},
                new AceTheme {ID = "ace/theme/monokai", Theme = "Monokai"},
            };
            aceThemes.ForEach(s => context.AceTheme.AddOrUpdate(p => p.ID, s));
            /*
            foreach (AceTheme item in aceThemes)
            {
                context.AceTheme.AddOrUpdate(s => s.ID == item.ID);
            }*/
            context.SaveChanges();

        }
    }
}
