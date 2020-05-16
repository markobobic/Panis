using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panis.Models
{
    public class ClientSector
    {
        [Key]
        public int ClientSectorID { get; set; }
        public string Name { get; set; }
        [ForeignKey("Sector")]
        public int SectorID { get; set; }
        public virtual Sector Sector { get; set; }


        
    }
}