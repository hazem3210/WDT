using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class FlightsClasses
    {
        public int ID { get; set; }

        public double Price { get; set; }

        [Display(Name ="Flight")]
        [Required]
        public int FlightId { get; set; }

        [ValidateNever]
        public Flight? Flight { get; set; }

        [Display(Name ="Class")]
        [Required]
        public int FlightClassId { get; set; }

        [ValidateNever]
        public FlightClass? FlightClass { get; set; }
    }
}
