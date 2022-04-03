using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Departing Gate")]
        public string DepartingGate { get; set; }

        [Required]
        [Display(Name = "Arrive Gate")]
        public string ArriveGate { get; set; }

        [Required]
        public string Class { get; set; }

        [Required]
        [Display(Name = "Number Of Stops")]
        public string NumberOfStops { get; set; }

        [Required]
        [Display(Name = "Date and Time")]
        public DateTime DateAndTime { get; set; }


        public int AirlineCode { get; set; }
        public Airline Airline { get; set; }

        public int DepartingAirportId { get; set; }
        [ForeignKey("DepartingAirportId")]
        public virtual Airport DepartingAirport { get; set; }

        public int ArrivingAirportId { get; set; }
        [ForeignKey("ArrivingAirportId")]
        public virtual Airport ArrivingAirport { get; set; }
    }
}
