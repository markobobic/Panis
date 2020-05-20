using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class RealizationType
    {
        [Key]
        public byte RealizationTypeID { get; set; }
        public string Name { get; set; }
    }
}