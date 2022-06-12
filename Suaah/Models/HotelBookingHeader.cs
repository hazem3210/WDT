using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class HotelBookingHeader
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Cancel Before(Hours)")]
        public int CancelBeforeHours { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } 

        public double TotalPrice { get; set; }

        public string Status { get; set; }
        public string Payment { get; set; }

        public string CustomerId { get; set; }
        [ValidateNever]
        public Customer Customer { get; set; }

        public IEnumerable<HotelBookingDetails> HotelBookingDetails { get; set; }
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; } 

    }
}
