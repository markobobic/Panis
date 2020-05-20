using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Panis.Models
{
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
        public int? CountNotifications { get; set; }
        public bool? ReadNotifications { get; set; }
        public WorkPosition? EmpPosition { get; set; }
        public enum WorkPosition
        {
            Developer = 1, QA = 2, HR = 3, Manager = 4
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeEnrollment> employeeEnrollments { get; set; }

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