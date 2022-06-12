using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class Airline
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Description { get; set; }
        [ValidateNever]
        [DisplayName("Image")]
        public string? ImageUrl { get; set; }

        [ValidateNever]

        public ICollection<Flight>? Flights { get; set; }

    }
}
