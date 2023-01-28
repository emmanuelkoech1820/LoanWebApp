using System;

namespace Apps.Core.Models
{
    public class BankTransferBinding
    {
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }
        public string PaymentReason { get; set; }
        public string BankId { get; set; }
        public string Narration { get; set; }
        public string DestinationName { get; set; }
        public string DestinationBankCode { get; set; }
        public string TransferType { get; set; }
        public string Currency { get; set; }

    }
    public class PayLoanBindingModel
    {
        public string Reference { get; set; }
        public string LoanId { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string Telco { get; set; } = "SAF";
        public string CountryCode { get; set; } = "KE";
        public string CallBackUrl { get; set; }
        public string ErrorCallBackUrl { get; set; }
    }
    public class STKCallback
    {
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public Additionalparameters AdditionalParameters { get; set; }
    }

    public class Additionalparameters
    {
        public string StatusDescription { get; set; }
        public string TelcoReference { get; set; }
        public string Telco { get; set; }
        public string OperationType { get; set; }
    }

}
}