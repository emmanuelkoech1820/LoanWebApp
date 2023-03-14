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
        Task<ServiceResponse<LoanAccount>> GetLoanRequest(string reference);
        Task<ServiceResponse<BankTransferRequest>> GetBankTransferRequest(string reference);
        Task<ServiceResponse<List<LoanAccount>>> GetLoanRequests(string profileId);
        Task<ServiceResponse<List<LoanAccount>>> GetLoanRequests();
        Task<ServiceResponse> ValidateRequest(BankTransferRequest request);
        Task<ServiceResponse> AddVehicle(VehicleBindingModel request);
        Task<ServiceResponse<BankTransferRequest>> InitiateRequest(BankTransferBinding request);
        Task<ServiceResponse<LoanAccount>> InitiateLoanRequest(LoanBindingModel request);
        Task<ServiceResponse<LoanAccount>> ApproveLoanRequest(LoanApproval request, string profileId);
        Task<ServiceResponse> Transfer(BankTransferRequest request, string profileId);
        Task UpdateRequest(BankTransferRequest request);
        string GenerateSignature(BankTransferRequest request);
        Task<ServiceResponse<List<Vehicle>>> GetVehicles(string profileId);
        Task<ServiceResponse> PayLoan(PayLoanBindingModel request, string profileId);
        Task<ServiceResponse> STKCallback(STKCallback request);
        Task<ServiceResponse<List<LoanAccount>>> GetAppliedLoans(int count);
    }
    public interface INyumbaniManager
    {
        Task<ServiceResponse<List<PropertyModel>>> GetAllProperty(string agentId);
        Task<ServiceResponse> AddProperty(PropertyBindingModel model);
        Task<ServiceResponse> UpdateProperty(string reference, bool status);

    }
}
