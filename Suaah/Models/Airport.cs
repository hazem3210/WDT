using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suaah.Models
{
    public class Airport
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter the Name")]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [Required(ErrorMessage = "You must enter the City")]
        public string City { get; set; }

        [Display(Name ="Country")]
        [Required(ErrorMessage = "You must choose a Country")]
        public int CountryId { get; set; }

        [ValidateNever]
        public Country? Country { get; set; }

        [ValidateNever]
        public virtual ICollection<Flight>? DepartingFlights { get; set; }
        [ValidateNever]
        public virtual ICollection<Flight>? ArrivingFlights { get; set; }
    }
}
