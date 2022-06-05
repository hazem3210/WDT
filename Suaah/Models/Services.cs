using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class Services
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ValidateNever]
        public ICollection<HotelRoomServices> HotelBookings { get; set; }
    }
}
