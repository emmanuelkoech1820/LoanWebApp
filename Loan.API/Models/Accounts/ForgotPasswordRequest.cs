using System.ComponentModel.DataAnnotations;

namespace WebApi.Consts.Accounts
{
    public class ForgotPasswordRequest
    {
       
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}