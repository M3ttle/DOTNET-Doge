using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using DOGEOnlineGeneralEditor.Models.POCO;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DOGEOnlineGeneralEditor.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public User User { get; set; }

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
        int SaveChanges();
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

            public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}