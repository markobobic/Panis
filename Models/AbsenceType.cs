using System.ComponentModel.DataAnnotations;

namespace Panis.Models
{
    public class AbsenceType
    {
        [Key]
        public int AbsenceTypeID { get; set; }
        public string Name { get; set; }
    }
}