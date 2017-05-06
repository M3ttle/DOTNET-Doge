using System;
using System.Linq;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using System.Data.Entity;

namespace DOGEOnlineGeneralEditor.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public void Initialize(ApplicationDbContext context)
        {
            //context.Database.EnsureCreated();
            // Look for any users.
            //if (context.ApplicationUser.Any())
            //{
            // DB has been seeded
            //    return;
            //}
        }
        /*
        protected override void Seed(ApplicationDbContext context)
        { 
            LanguageType[] languageTypes = new LanguageType[]
            {
                new LanguageType{Name = "Javascript"},
                new LanguageType{Name = "c#"},
                new LanguageType{Name = "html"},
            };

            foreach (LanguageType languageType in languageTypes)
            {
                context.LanguageType.Add(languageType);
            }

            context.SaveChanges();

            UserType[] userTypes = new UserType[]
            {
                new UserType{Name="Student"},
                new UserType{Name="Teacher"},
                new UserType{Name="Programmer"},
            };

            foreach (UserType userType in userTypes)
            {
                context.UserType.Add(userType);
            }
            
            context.SaveChanges();
            */
        /*
        ApplicationUser[] applicationUsers = new ApplicationUser[]
            { new ApplicationUser
                {
                    UserName = "Arnar",
                    Email = "arnart08@ru.is",
                    User = new User {Name = "Arnar", Email = "arnart08@ru.is"}
                    //Gender = "Male",
                    //DateJoined = DateTime.Parse("2000-01-01"),
                    //UserTypeID = 1


                },
            new ApplicationUser
                {
                    UserName = "Aþena",
                    Email = "Aþena08@ru.is",
                    User = new User {Name = "Aþena", Email = "Aþena@ru.is"}
                    //Gender = "Male",
                    //DateJoined = DateTime.Parse("2000-01-01"),
                    //UserTypeID = 3

                },
            new ApplicationUser
                {
                    UserName = "Mattías",
                    Email = "Mattías08@ru.is",
                    User = new User {Name = "Mattías", Email = "Mattías@ru.is"}
                    //Gender = "Male",
                    //DateJoined = DateTime.Parse("2000-01-01"),
                    //UserTypeID = 2

                },
            new ApplicationUser
                {
                    UserName = "Andri",
                    Email = "Andri08@ru.is",
                    User = new User {Name = "Andri", Email = "Andri@ru.is"}
                    //Gender = "Male",
                    //DateJoined = DateTime.Parse("2000-01-01"),
                    //UserTypeID = 1

                },
            new ApplicationUser
                {
                    UserName = "Jón",
                    Email = "Jón08@ru.is",
                    User = new User {Name = "Jón", Email = "Jón@ru.is"}
                    //Gender = "Male",
                    //DateJoined = DateTime.Parse("2000-01-01"),
                    //UserTypeID = 2

                }
            };




        context.SaveChanges();

        Project[] projects = new Project[]
        {
            new Project{Name="Doge",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Alpha",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Bravo",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Charlie",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Delta",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Enigma",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Faraday",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Groundhog",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },
            new Project{Name="Hellstorm",
                FileCount =1, DateCreated=DateTime.Parse("2000-01-01"),
                IsPublic =true, LanguageTypeID = 1, OwnerID = 1
                },

        };

        foreach (Project project in projects)
        {
            context.Project.Add(project);
        }

        context.SaveChanges();


        UserProject[] userProjects = new UserProject[]
        {
            new UserProject{UserID=1, ProjectID=1},
            new UserProject{UserID=1, ProjectID=2},
            new UserProject{UserID=1, ProjectID=3},
            new UserProject{UserID=1, ProjectID=4},
            new UserProject{UserID=1, ProjectID=5},
            new UserProject{UserID=1, ProjectID=6},
            new UserProject{UserID=2, ProjectID=3},
            new UserProject{UserID=2, ProjectID=4},
            new UserProject{UserID=2, ProjectID=6},
            new UserProject{UserID=2, ProjectID=7},
            new UserProject{UserID=2, ProjectID=9},
            new UserProject{UserID=3, ProjectID=5},
            new UserProject{UserID=3, ProjectID=6},
            new UserProject{UserID=3, ProjectID=7},
            new UserProject{UserID=3, ProjectID=1},
            new UserProject{UserID=4, ProjectID=7},
            new UserProject{UserID=4, ProjectID=8},
            new UserProject{UserID=4, ProjectID=9},
            new UserProject{UserID=4, ProjectID=1},
            new UserProject{UserID=5, ProjectID=9},
            new UserProject{UserID=5, ProjectID=4},
            new UserProject{UserID=5, ProjectID=6},
            new UserProject{UserID=5, ProjectID=2}

        };

        foreach (UserProject userProject in userProjects)
        {
            context.UserProject.Add(userProject);
        }

        context.SaveChanges();

        File[] files = new File[]
        {
            new File{Name="index.js",
                Location ="",DateCreated = DateTime.Parse("2000-01-01"),
                ProjectID = 1, LanguageTypeID=1}
        };

        foreach (File file in files)
        {
            context.File.Add(file);
        }
        context.SaveChanges();
      
    }*/

    }
}
