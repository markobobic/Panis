using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class Project
    {

        [Key]
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

        public Project()
        {
            this.Employees = new HashSet<Employee>();
        }
    }
}