using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apps.Core.Abstract
{
    public interface IBankTransferManager
    {
        Task<ServiceResponse<BankTransferRequest>> GetBankTransferRequest(string reference);
        Task<ServiceResponse<List<LoanAccount>>> GetLoanRequests(string reference = "", int StatusKey = 0);
        Task<ServiceResponse> ValidateRequest(BankTransferRequest request);
        Task<ServiceResponse> AddVehicle(VehicleBindingModel request);
        Task<ServiceResponse<BankTransferRequest>> InitiateRequest(BankTransferBinding request);
        Task<ServiceResponse<LoanAccount>> InitiateLoanRequest(LoanBindingModel request);
        Task<ServiceResponse<LoanAccount>> ApproveLoanRequest(LoanApproval request);
        Task<ServiceResponse> Transfer(BankTransferRequest request);
        Task UpdateRequest(BankTransferRequest request);
        string GenerateSignature(BankTransferRequest request);
    }
}
