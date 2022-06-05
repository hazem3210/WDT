using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class FlightClass
    {
        public int ID { get; set; }

        [Required]
        public string Class { get; set; }

        [ValidateNever]
        public ICollection<FlightBooking> FlightBooking { get; set; }

        [ValidateNever]
        public ICollection<FlightsClasses> Flights { get; set; }
    }
}
