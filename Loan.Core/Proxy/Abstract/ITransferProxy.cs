
using Apps.Core.Consts;
using Apps.Data.Entities;
using System.Threading.Tasks;

namespace Apps.Core.Proxy.Abstract
{
    public interface ITransferProxy
    {
        Task<ServiceResponse> Intrabank(BankTransferRequest request);
        Task<ServiceResponse> Interbank(BankTransferRequest request);
    }
}
