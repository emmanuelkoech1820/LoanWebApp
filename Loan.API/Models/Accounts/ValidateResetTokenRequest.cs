using System.ComponentModel.DataAnnotations;

namespace WebApi.Consts.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}