using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class TeamLead
    {
        [Key]
        public int TeamLeadID { get; set; }
        [ForeignKey("Employees")]
        public int? EmployeeID { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }


        public TeamLead()
        {
            this.Employees = new HashSet<Employee>();
        }

       
    }
}