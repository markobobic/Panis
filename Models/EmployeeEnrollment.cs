using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class EmployeeEnrollment
    {
        public enum Level
        {
            None = 0, Junior = 1, Medior=2, Senior=3
        }
       
        [Key]
        public int EmployeeEnrollmentID { get; set; }
        [Index]
        public DateTime? OfficialWorkStart { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WorkStart { get; set; }
        [DisplayFormat(NullDisplayText = "No seniority")]
        public Level? Seniority { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }  

    }
}