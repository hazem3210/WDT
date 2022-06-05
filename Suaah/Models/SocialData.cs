using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class SocialData
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data Name")]
        [StringLength(200, ErrorMessage = "Data Name cannot be longer than 200 characters.")]
        public string SocialName { get; set; }
        [Required]
        public string Link { get; set; }
    }
}
