using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class Airport
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }
        
        [Required]
        public string Governorate { get; set; }
        
        [Required]
        public string City { get; set; }
        
        [Required]
        public string Description { get; set; }


        public virtual ICollection<Flight> DepartingFlights { get; set; }
        public virtual ICollection<Flight> ArrivingFlights { get; set; }
    }
}
