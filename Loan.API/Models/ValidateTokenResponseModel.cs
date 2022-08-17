using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Consts
{
    public class ValidateTokenResponseModel
    {
        public string Token { get; set; }
        public DateTime? ExpiresIn { get; set; }
    }
}
