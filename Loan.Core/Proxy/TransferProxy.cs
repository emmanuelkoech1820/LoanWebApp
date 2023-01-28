
using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Core.Proxy.Abstract;
using Apps.Core.Utils;
using Apps.Data.Entities;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apps.Core.Proxy
{
    public class TransferManager: ITransferProxy
    {
        private IConfiguration _configuration;
        private HttpClientUtil _httpClient;
        private string _baseUrl;
        private string _url;
        private string _stkUrl;
        private readonly IHttpContextAccessor _httpAccessor;
        public TransferManager(IHttpContextAccessor httpAccessor, IConfiguration configuration, HttpClientUtil httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _url = $"{configuration["Proxy:BankTransfer"]}/intra";
            _httpAccessor = httpAccessor;
            _stkUrl = $"{configuration["Proxy:BankTransfer"]}/intra";
        }

        public async Task<ServiceResponse> Interbank(BankTransferRequest request)
        {

            try
            {
                var payload = new InterBankTransferModel
                {
                    BankId = request.BankId,
                    Amount = request.Amount,
                    DestinationAccount = request.DestinationAccount,
                    Narration = request.Narration,
                    PaymentReason = request.PaymentReason,
                    Reference = request.Reference,
                    SourceAccount = request.SourceAccount,
                    DestinationBankCode = request.DestinationBankCode,
                    DestinationName = request.DestinationName
                    
                };

                var accessToken = await GetAccessToken();
                var header = new Dictionary<string, string>();
                header.Add("Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}");
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_url}", payload: payload, headers: header);
                return response;
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return new ServiceResponse { StatusMessage = message.StatusMessage, StatusCode = message.StatusCode};
            }
        }

        public async Task<ServiceResponse> Intrabank(Data.Entities.BankTransferRequest request)
        {

            try
            {
                var payload = new IntraBankTransferModel
                {
                    BankId = request.BankId,
                    Amount = request.Amount,
                    DestinationAccount = request.DestinationAccount,
                    Narration = request.Narration,
                    PaymentReason = request.PaymentReason,
                    Reference = request.Reference,
                    SourceAccount = request.SourceAccount
                };

                var accessToken = await GetAccessToken();
                var header = new Dictionary<string, string>();
                header.Add("Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}");               
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_url}", payload: payload, headers: header);
                return response;
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return new ServiceResponse { StatusMessage = message.StatusMessage, StatusCode = message.StatusCode };
            }
        }
        private async Task<TokenResponseModel> GetAccessToken()
        {
            var client_id = _configuration["Proxy:ClientId"];
            var client_secret = _configuration["Proxy:ClientSecret"];
            var grant_type = "client_credentials";
            var payload = $"client_id={ client_id}&client_secret={client_secret}&grant_type={grant_type}";
            var url = $"{_configuration["Proxy:TokenURL"]}";
            var header = new Dictionary<string, string>();
            header.Add("Content-type", "application/x-www-form-urlencoded");
            var response = await _httpClient.PostUrlEncodedAsync<TokenResponseModel>(url, payload, header);
            return response;
        }

        public async Task<ServiceResponse> PayLoan(PayLoanBindingModel request)
        {
            try
            {
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_stkUrl}", payload: request);
                return response;
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return new ServiceResponse { StatusMessage = message.StatusMessage, StatusCode = message.StatusCode };
            }
        }
        public async Task<ServiceResponse> SendSms(PayLoanBindingModel request)
        {
            try
            {
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_stkUrl}", payload: request);
                return response;
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return new ServiceResponse { StatusMessage = message.StatusMessage, StatusCode = message.StatusCode };
            }
        }
    }
}
