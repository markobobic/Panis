using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panis.Models
{
    public class EmployeesProject
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Employee")]
        public int? EmployeeID { get; set; }
        [ForeignKey("Project")]
        public int? ProjectID { get; set; }


        public Employee Employee { get; set; }
        public Project Project { get; set; }

    }
}