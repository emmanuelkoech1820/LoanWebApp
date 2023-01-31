using CApps.Dataore.Entities;
using System;
using System.Collections.Generic;

namespace Apps.Data.Entities
{
    public class Otp : BaseModel
    {
        public string Reference { get; set; }
        public string Operation { get; set; }
        public string Source { get; set; }
        public byte[] Password { get; set; }
        public int NoOfDigits { get; set; }
        public string To { get; set; }
        public string Platform { get; set; }
        public DateTime ExpiresOn { get; set; }
        public int RetryAttempts { get; set; }
        public int NoOfRegenerations { get; set; }
        public DateTime? UsedOn { get; set; }
        public string? Status { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }

    public enum RecipientPlatform
    {
        Sms = 1,
        Email
    }

    public class OtpStatus
    {
        public const string Generated = "Generated";
        public const string Regenerated = "Regenerated";
        public const string Success = "Success";
        public const string Invalid = "Invalid";
    }
}