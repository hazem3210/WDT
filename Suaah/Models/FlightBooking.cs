using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class FlightBooking
    {
        [Key]
        public int Booknum { get; set; }

        [Required]
        [Column("NoOfSeats")]
        [Display(Name = "Number Of Seats")]
        [Range(1,int.MaxValue,ErrorMessage ="Enter valid number")]
        public int NumberOfSeats { get; set; }

        
        [Display(Name = "Total Price")]
        public double? TotalPrice { get; set; }

        [Display(Name = "Flight")]
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        [Display(Name = "Class")]
        public int FlightClassId { get; set; }
        public FlightClass? FlightClass { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }

    }
}
