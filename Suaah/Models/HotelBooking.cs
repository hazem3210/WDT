using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class HotelBooking
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [Column("NoOfRooms")]
        [Display(Name = "Number Of Rooms")]
        public string NumberOfRooms { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Column("NoOfDays")]
        [Display(Name = "Number Of Days")]
        public int NumberOfDays { get; set; }

        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }

        public int HotelRoomlId { get; set; }
        public HotelRoom HoteRoom { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
