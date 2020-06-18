using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using System.IO;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Principal;
using System.Text;

namespace Panis.Models
{
    public static class IdentityExtensions
    {
        public static string FullUserName(this IIdentity identity)
        {
            var claim = identity as ClaimsIdentity;
            return claim.FindFirst("UserFullName") == null ?
                     string.Empty :
                    claim.FindFirst("UserFullName").Value;
        }

        public static string UserImagePath(this IIdentity identity)
        {
            var claim = identity as ClaimsIdentity;
            return claim.FindFirst("UserImagePath").Value;
        }
    }

    public static class HtmlExtensions
    {
        public static MvcHtmlString UserImage(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            if (File.Exists(HttpContext.Current.Server.MapPath(Path.Combine("~", HttpContext.Current.User.Identity.UserImagePath()))))
                sb.Append("<img src=\"Content/Images/UsersImage/imgDefault.jpg\" class=\"img-circle user-img\" alt=\"User Image\" width=\"22\" height=\"22\" />");
            else
                sb.Append(string.Format("<img src=\"{0}\" class=\"img-circle user-img\" alt=\"User Image\" width=\"22\" height=\"22\" />", HttpContext.Current.User.Identity.UserImagePath()));
            return MvcHtmlString.Create(sb.ToString());
        }
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [NotMapped]
        [Display(Name = "User roles ")]
        public string UserRoles { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CountNotifications { get; set; }
        public bool? ReadNotifications { get; set; }
        public WorkPosition? EmpPosition { get; set; }
        public string ImagePath { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public enum WorkPosition
        {
            Developer = 1, QA = 2, HR = 3, Manager = 4
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("UserFullName", string.Format("{0} {1}", FirstName, LastName)));
            userIdentity.AddClaim(new Claim("UserImagePath", string.IsNullOrEmpty(ImagePath) ? "/Images/UsersImage/imgDefault.jpg" : ImagePath));
            return userIdentity;
        }
    }
    public class UsersChat
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string FromUserId { get; set; }
        public string OtherUserId { get; set; }
        public string RoomId { get; set; }
        public string ClientGuidId { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeEnrollment> employeeEnrollments { get; set; }
        public DbSet<UsersChat> UsersChat { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Realization> Realizations { get; set; }
        public DbSet<RealizationType> RealizationTypes { get; set; }
        public DbSet<ClientSector> ClientSectors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<TeamLead> TeamLeads { get; set; }
        public DbSet<EmployeesProject> EmployeesProjects { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<AbsenceType> AbsenceTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().Ignore(x => x.ImagePath);
            modelBuilder.Entity<Employee>().Ignore(x => x.FullName);
           
        }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       
    }
}