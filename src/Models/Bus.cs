using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Bus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BusID { get; set; }

        public string BusNumber { get; set; }

        public ICollection<BusLocation> BusLocations { get; set; }
    }
}