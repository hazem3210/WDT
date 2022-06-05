using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Suaah.Models
{
    public class HotelRoomServices
    {
        public int HotelRoomId { get; set; }
        [ValidateNever]
        public HotelRoom HotelRoom { get; set; }

        public int ServicesId { get; set; }
        [ValidateNever]
        public Services Services { get; set; }
    }
}
