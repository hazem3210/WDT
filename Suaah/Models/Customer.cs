using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class Customer
    {
        [Key]
        [ForeignKey("IdentityUser")]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }
        
        [Required]
        [Display(Name = "Citizenship Country")]
        public string CitizenshipCountry { get; set; }
        
        [Required]
        [Display(Name = "Residence Country")]
        public string ResidenceCountry { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [ValidateNever]
        public ICollection<HotelBooking> HotelBookings { get; set; } 
        [ValidateNever]
        public ICollection<HotelBookingHeader> HotelBookingHeaders { get; set; }
        [ValidateNever]
        public ICollection<FlightBooking> FlightBookings { get; set; }

        [ValidateNever]
        public IdentityUser IdentityUser { get; set; }
    }
}
