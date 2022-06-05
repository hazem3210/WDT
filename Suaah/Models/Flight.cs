using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter the Departing Gate.")]
        [MinLength(4,ErrorMessage = "The Departing Gate should be bigger than 4 chars.")]
        [Display(Name = "Departing Gate")]
        public string DepartingGate { get; set; }

        [Required(ErrorMessage = "You must enter the Arrive Gate.")]
        [MinLength(4, ErrorMessage = "The Arrive Gate should be bigger than 4 chars.")]
        [Display(Name = "Arrive Gate")]
        public string ArriveGate { get; set; }

        [Required(ErrorMessage = "You must enter Number Of Stops.")]
        [Range(0,6, ErrorMessage = "Enter a valid number.")]
        [Display(Name = "Number Of Stops")]
        public string NumberOfStops { get; set; }

        [Required(ErrorMessage = "You must enter the Take off Time.")]
        [Display(Name = "Take off Time")]
        public DateTime LeaveTime { get; set; }

        [Required(ErrorMessage = "You must choose an Airline.")]
        [Display(Name = "Airline")]
        public string AirlineCode { get; set; }

        [ValidateNever]
        [ForeignKey("AirlineCode")]
        public Airline? Airline { get; set; }

        [Required(ErrorMessage = "You must choose a Departing Airport.")]
        [Display(Name = "Departing Airport")]
        public int DepartingAirportId { get; set; }

        [ValidateNever]
        [ForeignKey("DepartingAirportId")]
        public virtual Airport? DepartingAirport { get; set; }

        [Required(ErrorMessage = "You must choose a Arriving Airport.")]
        [Display(Name = "Arriving Airport")]
        public int ArrivingAirportId { get; set; }

        [ValidateNever]
        [ForeignKey("ArrivingAirportId")]
        public virtual Airport? ArrivingAirport { get; set; }

        [ValidateNever]
        [Display(Name = "Classes")]
        public ICollection<FlightsClasses>? FlightClasses { get; set; }

    }
}
