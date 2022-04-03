using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
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

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0,9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        public ICollection<HotelBooking> HotelBookings { get; set; }
        public ICollection<FlightBooking> FlightBookings { get; set; }
    }
}
