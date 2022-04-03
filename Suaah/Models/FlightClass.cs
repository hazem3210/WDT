namespace Suaah.Models
{
    public class FlightClass
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public double Price { get; set; }

        public int FlightId { get; set; }
        public Flight Flight { get; set; }

        public ICollection<FlightBooking> FlightBooking { get; set; }
    }
}
