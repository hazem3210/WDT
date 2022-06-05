namespace Suaah.Models
{
    public class HotelBookingHeaderAndDetails
    {
        public HotelBookingHeader HotelBookingHeader { get; set; } = new HotelBookingHeader();
        public List<HotelBookingDetails> HotelBookingDetails { get; set; } = new List<HotelBookingDetails>();
        public List<HotelBookingHeader> HotelBookingHeaders { get; set; } = new List<HotelBookingHeader>();
    }
}
