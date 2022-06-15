using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace Suaah.Models
{
    public class HotelBooking
    {
        public int Id { get; set; }

        [Display(Name = "Check In")]
        [CheckinValidation(ErrorMessage = "Enter Check In date and time at least five hours from now")]
        public DateTime Date { get; set; }

        [Column("NoOfDays")]
        [Display(Name = "Number Of Days")]
        [Range(1, int.MaxValue)]
        public int NumberOfDays { get; set; }

        [Column("NoOfRooms")]
        [Display(Name = "Number Of Rooms")]
        [Range(1, int.MaxValue)]
        public int NumberOfRooms { get; set; } = 1;

        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }

        public string NotCustomerId { get; set; } = "";

        public int Flag { get; set; } = 0;

        public int HotelRoomId { get; set; }
        [ValidateNever]
        public HotelRoom HoteRoom { get; set; }

        [Display(Name = "Customer")]

        public string? CustomerId { get; set; }
        [ValidateNever]
        public Customer Customer { get; set; }
    }
}
