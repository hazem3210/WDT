using System;
using System.ComponentModel.DataAnnotations;

namespace Suaah.Models
{
    public class CheckinValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            HotelBooking hotelBooking = (HotelBooking)validationContext.ObjectInstance;

            TimeSpan timeSpan = hotelBooking.Date - DateTime.Now;

            if (timeSpan.Hours < 5)
            {
                return new ValidationResult(ErrorMessage);
            }

            if (hotelBooking.Date < DateTime.Now)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
