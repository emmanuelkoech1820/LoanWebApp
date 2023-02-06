using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class LoanAccount : BaseModel
    {
        [ForeignKey("Accounts")]
        public string ProfileId { get; set; }
        public DisbursmentStatus DisbursmentStatus { get; set; }
        //public LoanApprovalStatus LoanAprroved { get; set; }
        public bool LoanAprroved { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal RepaidAmount { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal LoanBalance { get; set; }
        public string RepaymentStatus { get; set; }
        public string Currency { get; set; }
        public string DestinationAccount { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string LoanReason { get; set; }
        public string DestinationBankCode { get; set; }
        public int RepaymentPeriod { get; set; }
        public string DestinationName { get; set; }
        public string Narration { get; set; }
        public string VehicleReferenceNumber { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public DateTime LoanRepaymentDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public virtual ICollection<LoanHistory> LoanHistories { get; set; }
    }
    public class LoanHistory : BaseModel
    {
        public string Action { get; set; }
        public string Description { get; set; }
        public string PerformedBy { get; set; } // Unique ID of 
        [ForeignKey("LoanAccount")]
        public int LoanRequest { get; set; }
        public virtual LoanAccount LoanAccount { get; set; }
        public decimal BorrowedAmount { get; set; }
        public decimal RepaidAmount { get; set; }
    }

    public enum DisbursmentStatus
    {
        Pending,
        Disbursed,
        Rejected
    }
    public enum LoanApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }
}