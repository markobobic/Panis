using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panis.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Index]
        public string LastName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        [Index]
        public string FirstName { get; set; }
        public string Education { get; set; }
        public string Mobile { get; set; }
        public string ReportsTo { get; set; }
        public string PhotoType { get; set; }
        public byte[] Photo { get; set; }
        public string LivingCity { get; set; }
        public string LivingStreet { get; set; }
        public string LivingStreetNumber { get; set; }
        public string CityFromID { get; set; }
        public string StreetFromID { get; set; }
        public string StreetNumberFromID { get; set; }
        public string AppartmentNumberFromID { get; set; }
        public string ImagePath { get; set; }

        [ForeignKey("TeamLead")]
        public int? TeamLeadID { get; set; }
        [ForeignKey("Sector")]
        public int? SectorID { get; set; }
        [ForeignKey("ClientSector")]
        public int? ClientSectorID { get; set; }
        [ForeignKey("Department")]
        public int? DepartmentID { get; set; }
        public virtual TeamLead TeamLead { get; set; }
        public virtual ClientSector ClientSector { get; set; }
        public virtual Sector Sector { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public bool IsTeamLead { get; set; }

        public string FullName
        {
            get
            {
                return LastName + " " + FirstName;
            }
        }
        public Employee()
        {
            this.Projects = new HashSet<Project>();
        }
       
    }
}