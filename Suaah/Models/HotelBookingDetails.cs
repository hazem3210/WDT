using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class HotelBookingDetails
    {
        public int Id { get; set; }

        [Display(Name = "Check In")]
        [CheckinValidation(ErrorMessage = "Enter Check In date and time at least five hours from now")]
        public DateTime CheckInDate { get; set; }

        [Column("NoOfDays")]
        [Display(Name = "Number Of Days")]
        public int NumberOfDays { get; set; }

        [Column("NoOfRooms")]
        [Display(Name = "Number Of Rooms")]
        public int NumberOfRooms { get; set; } = 1;

        [Display(Name = "Price For a Day")]
        public double PriceForDay { get; set; }

        public int HotelBookingHeaderId { get; set; }
        [ValidateNever]
        public HotelBookingHeader HotelBookingHeader { get; set; }

        public int HotelRoomId { get; set; }
        [ValidateNever]
        public HotelRoom HoteRoom { get; set; }
    }
}
