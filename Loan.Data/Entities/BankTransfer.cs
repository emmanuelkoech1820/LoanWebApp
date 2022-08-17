using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class BankTransferRequest : BaseModel
    {
        [ForeignKey("Accounts")]
        public string ProfileId { get; set; }
        [ForeignKey("Accounts")]
        public string LoanRequestId { get; set; }
        public string SourceAccount { get; set; }
        public string DestinationAccount { get; set; }
        public string TransferType { get; set; }
        public string DestinationBankCode { get; set; }
        public string DestinationName { get; set; }
        public string Narration { get; set; }
        public string PaymentReason { get; set; }
        public string Reference { get; set; }

        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string BankId { get; set; }
        public string Currency { get; set; }
        public virtual ICollection<History> Histories { get; set; }
    }
    public class History : BaseModel
    {
        public string Action { get; set; }
        public string Description { get; set; }
        public string PerformedBy { get; set; } // Unique ID of 
        [ForeignKey("BankTransferRequest")]
        public int BankTransferRequestId { get; set; }
        public virtual BankTransferRequest BankTransferRequest { get; set; }
    }
}