using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class AbsenceOfEmployee
    {
        [Key]
        public int AbsenceLimitID { get; set; }
        public short Year { get; set; }
        public byte NumberOfVacations { get; set; }
        public short NumberOfSickLeave { get; set; }
        public byte NumberOfFreeDays { get; set; }
    }
}