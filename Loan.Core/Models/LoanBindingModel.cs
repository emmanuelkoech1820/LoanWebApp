using Apps.Data.Entities;
using System;

namespace Apps.Core.Models
{
    public class LoanBindingModel
    {
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string DestinationAccount { get; set; }
        public string DestinationBankCode { get; set; }
        public string DestinationName { get; set; }

        public int RepaymentPeriod { get; set; }
        public string Narration { get; set; }
        public string LoanReason { get; set; }
        public string ProfileId { get; set; }
        public string RegistrationNumber { get; set; }
        public string VehicleReferenceNumber { get; set; }
        public string VehicleRegistrationNumber { get; set; }
    }
    public class LoanApproval : LoanBindingModel
    {
        public string SourceAccount { get; set; }
        public bool LoanApprovalStatus { get; set; }

        public DisbursmentStatus Status { get; set; }
       
    }
    public class VehicleBindingModel
    {
        public string Reference { get; set; }
        public string VehicleCategory { get; set; }
        public string ProfileId { get; set; }
        public string VehicleType { get; set; }
        public string InsuranceCoverType { get; set; }
        public string VehicleModel { get; set; }
        public string YearOfManufacture { get; set; }

        public decimal VehicleValue { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime InsturanceStartDate { get; set; }
    }
}