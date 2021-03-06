﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DOGEOnlineGeneralEditor.Models.POCO;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOGEOnlineGeneralEditor.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public interface IAppDataContext
    {
        IDbSet<Project> Project { get; set; }
        IDbSet<UserProject> UserProject { get; set; }
        IDbSet<User> User { get; set; }
        IDbSet<LanguageType> LanguageType { get; set; }
        IDbSet<File> File { get; set; }
        IDbSet<UserType> UserType { get; set; }
        IDbSet<AceTheme> AceTheme { get; set; }
        int SaveChanges();
        System.Data.Entity.Infrastructure.DbEntityEntry Entry(object entity);
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IAppDataContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        { 
            
        }
        
        public IDbSet<Project> Project { get; set; }
        public IDbSet<UserProject> UserProject { get; set; }
        public IDbSet<User> User { get; set; }
        public IDbSet<LanguageType> LanguageType { get; set; }
        public IDbSet<File> File { get; set; }
        public IDbSet<UserType> UserType { get; set; }
        public IDbSet<AceTheme> AceTheme { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<AceTheme>().Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

            public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}