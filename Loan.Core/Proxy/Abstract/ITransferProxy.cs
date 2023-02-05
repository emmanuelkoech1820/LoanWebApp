
using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Data.Entities;
using System.Threading.Tasks;

namespace Apps.Core.Proxy.Abstract
{
    public interface ITransferProxy
    {
        Task<(IntraBankTransferModel request, ServiceResponse)> Intrabank(BankTransferRequest request);
        Task<(IntraBankTransferModel request, ServiceResponse)> Interbank(BankTransferRequest request);
        Task<ServiceResponse> PayLoan(PayLoanBindingModel request);
    }
    
}
