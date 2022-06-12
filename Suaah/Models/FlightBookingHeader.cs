using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class FlightBookingHeader
    {
        public int ID { get; set; }

        [Display(Name = "Customer")]
        public string CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }

        [Display(Name = "Order Status")]
        public string? OrderStatus { get; set; }

        [Display(Name = "Payment Status")]
        public string? PaymentStatus { get; set; }

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Payment Due Date")]
        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentId { get; set; }


        [NotMapped]
        public ICollection<FlightBookingDetails>? FlightBookings { get; set; }

    }
}
