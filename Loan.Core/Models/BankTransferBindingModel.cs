using Apps.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Apps.Core.Models
{
    public class BankTransferBinding
    {
        [Required]
        public string Reference { get; set; }
        [Required]
        public string LoanReference { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }
        public string PaymentReason { get; set; }
        public string BankId { get; set; }
        public string Narration { get; set; }
        public string DestinationName { get; set; }
        public string DestinationBankCode { get; set; }
        public string TransferType { get; set; }
        public string Currency { get; set; }
        [Required]
        public LoanStatus LoanStatus { get; set; }

    }
    public class PayLoanBindingModel
    {
        [Required]
        public string Reference { get; set; }
        [Required]
        public string LoanReference { get; set; }
        [Required]
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
    public class TransactingAccount
    {
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankId { get; set; }
        public string AccountClass { get; set; }
        public string TransferType { get; set; }
        public string AccountName { get; set; }
    }

}
