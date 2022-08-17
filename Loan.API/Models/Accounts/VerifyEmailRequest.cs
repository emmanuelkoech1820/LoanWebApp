using System.ComponentModel.DataAnnotations;

namespace WebApi.Consts.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}