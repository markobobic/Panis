using System;
using System.ComponentModel.DataAnnotations;

namespace Panis.Models
{
    public class AbsenceOfEmployee
    {
        [Key]
        public int AbsenceLimitID { get; set; }
        public DateTime Year { get; set; }
        public int NumberOfVacations { get; set; }
        public int NumberOfSickLeave { get; set; }
        public int NumberOfFreeDays { get; set; }
    }
}