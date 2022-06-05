using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class FlightBookingDetails
    {
        public int Id { get; set; }

        [Column("NoOfSeats")]
        [Display(Name = "Number Of Seats")]
        public int NumberOfSeats { get; set; }

        public int OrderID { get; set; }

        [ForeignKey("OrderID")]
        public FlightBookingHeader? Order { get; set; }

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
