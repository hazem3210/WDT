using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class HotelRoom
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }


        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
