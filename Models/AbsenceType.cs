using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class AbsenceType
    {
        [Key]
       
        public byte AbsenceTypeID { get; set; }
        public string Name { get; set; }
    }
}