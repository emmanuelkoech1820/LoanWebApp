using System.ComponentModel.DataAnnotations;

namespace WebApi.Consts.Accounts
{
    public class AuthenticateRequest
    {
        
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}