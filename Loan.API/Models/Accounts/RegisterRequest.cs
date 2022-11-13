using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Consts.Accounts
{
    public class RegisterRequest
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(4), MaxLength(4)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Numbers allowed.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
        public bool AccountVerified { get; set; }

        [Required]
        public string IdNumber { get; set; }
        public string EmployerName { get; set; }
        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public string ClientId { get; set; }
    }
}