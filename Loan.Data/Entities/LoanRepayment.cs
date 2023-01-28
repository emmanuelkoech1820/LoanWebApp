using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class LoanRepayment : BaseModel
    {
        [ForeignKey("Accounts")]
        public string ProfileId { get; set; }
        public string LoanId { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public string SourcePhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public RepaymentStatus Status { get; set; }
        public string JsonResponse { get; set; }
        public string JsonRequest { get; set; }
    }

    public enum RepaymentStatus
    {
        STKPushReceived = 1000,
        STKPushSent,
        Succesfull,
        Failed
    }

}