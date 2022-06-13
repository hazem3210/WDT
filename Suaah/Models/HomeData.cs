using Microsoft.EntityFrameworkCore;

namespace Suaah.Models
{
    public class HomeData
    {
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Country> flightsCountries { get; set; }

        public IEnumerable<Airline> Airlines { get; set; }
        public IEnumerable<Airport> Airports { get; set; }
        public IEnumerable<Hotel> Hotels { get; set; }
    }
}
