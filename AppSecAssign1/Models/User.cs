using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlexAppSecAssign.Models
{
    public class User
    {

        public string Id { get; set; }

        //[Required]
        public string FirstName { get; set; }

        //[Required]
        public string LastName { get; set; }

        //[Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Provide a valid email address")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$", 
            ErrorMessage = "Password needs a combination of at least 12 lower-case, upper-case, numbers & special characters")]
        public string Password { get; set; }

        // [Required]
        [DataType(DataType.Date)]
        public String DOB { get; set; }

        //[Required]
        [FileExtensions(Extensions = "jpg, jpeg, png", ErrorMessage = "Provide a valid File Type (JPG/PNG)")]
        [DataType(DataType.Upload)]
        public string Image { get; set; }

        //[Required]
        [CreditCard]
        public string CardNumber { get; set; }

        //[Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Provide a valid Card Expiry (DDMM)")]
        public string CardExpiry { get; set; }

        //[Required]
        [MaxLength(3, ErrorMessage = "Provide a valid CVV (3 digits)")]
        public string CardCVV { get; set; }

        public byte[] encKey { get; set; }

        public byte[] encIV { get; set; }

        public Boolean AccLockOut { get; set; }

        public string PasswordHistory { get; set; }

        public DateTime PasswordChangeTime { get; set; }

        public DateTime AccLockoutTime { get; set; }

        public int FailedLoginAttempts { get; set; }

        public DateTime OTPIssuedTime { get; set; }

        public string OTP { get; set; }
    }
}
