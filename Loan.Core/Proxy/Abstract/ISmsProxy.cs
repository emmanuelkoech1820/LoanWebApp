using Apps.Core.Consts;
using System.Threading.Tasks;

namespace Loan.Core.Proxy.Abstract
{
    public interface ISmsProxy
    {
        Task<ServiceResponse> SendSMS(string phoneNumber, string message, string operation);
    }
}