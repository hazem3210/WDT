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

        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        public int FlightClassId { get; set; }
        public FlightClass? FlightClass { get; set; }

        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }

    }
}
