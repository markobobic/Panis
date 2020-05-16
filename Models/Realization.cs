    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panis.Models
{
    public class Realization
    {
        [Key]
        public int RealizationID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        [ForeignKey("RealizationType")]
        public int? RealizationTypeID { get; set; }
        public RealizationType RealizationType { get; set; }

        [ForeignKey("Project")]
        public int? ProjectID { get; set; }
        public Project Project { get; set; }
        [ForeignKey("Department")]
        public int? DepartmentID { get; set; }
        public Department Department { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public int Hours { get; set; }
       

    }
}