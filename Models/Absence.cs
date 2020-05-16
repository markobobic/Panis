using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Panis.Models
{
    public class Absence
    {
        [Key]
        public int AbsenceID { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        [ForeignKey("AbsenceType")]
        public int? AbsenceTypeID { get; set; }
        public AbsenceType AbsenceType { get; set; }
        [ForeignKey("AbsenceOfEmployee")]
        public int? AbsenceOfEmployeeID { get; set; }
        public AbsenceOfEmployee AbsenceOfEmployee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ApplicationDate { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public bool Approved { get; set; }
    }
}