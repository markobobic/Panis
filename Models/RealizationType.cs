using System.ComponentModel.DataAnnotations;

namespace Panis.Models
{
    public class RealizationType
    {
        [Key]
        public int RealizationTypeID { get; set; }
        public string Name { get; set; }
    }
}