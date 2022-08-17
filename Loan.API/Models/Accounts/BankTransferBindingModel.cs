using System;

namespace Apps.Core.Models
{
    public class BankTransferBindingModels
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

    }
}