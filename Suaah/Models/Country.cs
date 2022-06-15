using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Suaah.Models
{
    public class Country
    {
        public int ID { get; set; }

        [Required(ErrorMessage ="You must enter the name")]
        public string Name { get; set; }

        [Display(Name ="Photo of a famous Place in The Country")]
        public string? PhotoPath { get; set; }

        [NotMapped]
        [ValidateNever]
        public IFormFile? Photo { get; set; }

        [Display(Name = "Flag Photo")]
        public string? FlagPath { get; set; }

        [NotMapped]
        [ValidateNever]
        public IFormFile? FlagPhoto { get; set; }

        [NotMapped]
        [ValidateNever]
        public virtual ICollection<Airport> Airports { get; set; }

        [NotMapped]
        [ValidateNever]
        public virtual ICollection<Hotel> Hotels { get; set; }

    }
}
