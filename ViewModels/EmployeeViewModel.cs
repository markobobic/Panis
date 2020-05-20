using Panis.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panis.ViewModels
{
    public class EmployeeViewModel
    {
        public int? EmployeeID { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WorkStart { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OfficialWorkStart { get; set; }

        public string LivingCity { get; set; }
        public string LivingStreet { get; set; }
        public string LivingStreetNumber { get; set; }
        public string CityFromID { get; set; }
        public string StreetFromID { get; set; }
        public string StreetNumberFromID { get; set; }
        public string AppartmentNumberFromID { get; set; }
        public string ReportsTo { get; set; }

        public string Education { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string UserRole { get; set; }
        public bool IsActive { get; set; }

        public bool IsTeamLead { get; set; }
        public int? TeamLeadID { get; set; }
        public int? SectorID { get; set; }
        public int? ClientSectorID { get; set; }

        public string Email { get; set; }
        public string Mobile { get; set; }
        public Level? Positions { get; set; }
        public Position EmployeePositions { get; set; }
        public enum Level
        {
            None = 1,
            [Display(Name = "Junior")]
            Junior = 2,
            [Display(Name = "Medior")]
            Medior = 3,
            [Display(Name = "Senior")]
            Senior = 4
        }
        public enum Position
        {
            [Display(Name ="Developer")]
            Developer = 1,
            [Display(Name = "QA")]
            QA = 2,
            [Display(Name = "HR")]
            HR = 3,
            [Display(Name = "Manager")]
            Manager = 4
        }
    }
}
