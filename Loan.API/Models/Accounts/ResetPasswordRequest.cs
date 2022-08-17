using System.ComponentModel.DataAnnotations;

namespace WebApi.Consts.Accounts
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(4), MaxLength(4)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only Numbers allowed.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}