using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class FlightBookingHeader
    {
        public int ID { get; set; }

        public string CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentId { get; set; }


        [NotMapped]
        public ICollection<FlightBookingDetails>? FlightBookings { get; set; }

    }
}
