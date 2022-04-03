using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class FlightBooking
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [Column("NoOfAdults")]
        [Display(Name = "Number Of Adults")]
        public string NumberOfAdults { get; set; }

        [Required]
        [Column("NoOfChildren")]
        [Display(Name = "Number Of Children")]
        public string NumberOfChildren { get; set; }

        [Required]
        [Column("NoOfInfants")]
        [Display(Name = "Number Of Infants")]
        public string NumberOfInfants { get; set; }

        [Required]
        [Column("NoOfSeats")]
        [Display(Name = "Number Of Seats")]
        public string NumberOfSeats { get; set; }

        [Required]
        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }


        public int FlightClassId { get; set; }
        public FlightClass FlightClass { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

    }
}
