using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class HotelRoom
    {
        public int Id { get; set; }
        //[Required]
        //public string Type { get; set; }

        [Required]
        public double Price { get; set; } 
        [Required]
        [Display(Name = "Cancel Before(Hours)")]
        public int CancelBeforeHours { get; set; }

        [Required]
        public string Description { get; set; }


        public int HotelId { get; set; }
        [ValidateNever]
        public Hotel Hotel { get; set; }

        [ValidateNever]
        public ICollection<HotelRoomServices> Services { get; set; }
    }
}
