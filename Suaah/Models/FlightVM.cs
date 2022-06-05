using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class FlightVM
    {

        public Flight Flight { get; set; }
        [Required(ErrorMessage ="You must choose the flight classes")]
        public List<int> Classes { get; set; }
        [Required(ErrorMessage = "You must enter the classes prices")]
        public List<double> Prices { get; set; }
    }
}
