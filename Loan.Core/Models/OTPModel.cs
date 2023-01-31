using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apps.Core.Models.OTPModel
{
    public class OtpMessage
    {
        [Required]
        public string Reference { get; set; }
        [Required]
        public string To { get; set; }
        [Required]
        public RecipientPlatform Platform { get; set; }
        [Required]
        public string Operation { get; set; }
        public string Source { get; set; }
    }
    public enum RecipientPlatform
    {
        Sms = 1,
        Email
    }
    public class OtpConfiguration
    {
        public int NumberOfRetries { get; set; }
        public int NumberOfRegenerations { get; set; }
        //public int ExpiresInMin { get; set; }
        public int ExpiresInMin { get; set; }
        public string NotificationTemplate { get; set; }
        public string Institution { get; set; }
        public int NoOfDigit { get; set; } 
    }
    public class OtpVerifyMessage
    {
        [Required]
        public string Reference { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Operation { get; set; }
        public string Source { get; set; }
    }


}
