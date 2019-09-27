using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class BusLocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BusLocationID { get; set; }
        [ForeignKey("FK__BusLocati__BusID__38996AB5")]
        public int BusID { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string StopID { get; set; }

        public DateTime LogTime { get; set; }

        public Bus Bus { get; set; }
    }
}