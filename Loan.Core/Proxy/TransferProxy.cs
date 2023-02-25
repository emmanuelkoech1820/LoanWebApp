
using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Core.Models.STKResponseModel;
using Apps.Core.Proxy.Abstract;
using Apps.Core.Utils;
using Apps.Data.Entities;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
        private readonly ILogger<TransferManager> _logger;
        public TransferManager(ILogger<TransferManager> logger, IHttpContextAccessor httpAccessor, IConfiguration configuration, HttpClientUtil httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _url = $"{configuration["Proxy:BankTransfer"]}/intra";
            _httpAccessor = httpAccessor;
            _stkUrl = $"{configuration["Proxy:STKPushUrl"]}";
        }

        public async Task<(IntraBankTransferModel request, ServiceResponse)> Interbank(BankTransferRequest request)
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


            try
            {
               
                var accessToken = await GetAccessToken();
                var header = new Dictionary<string, string>();
                header.Add("Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}");
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_url}", payload: payload, headers: header);
                return (payload, response);
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return (payload, new ServiceResponse { StatusMessage = message.StatusMessage, StatusCode = message.StatusCode});
            }
        }

        public async Task<(IntraBankTransferModel request, ServiceResponse)> Intrabank(Data.Entities.BankTransferRequest request)
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

            try
            {
                

                var accessToken = await GetAccessToken();
                var header = new Dictionary<string, string>();
                header.Add("Authorization", $"{accessToken.TokenType} {accessToken.AccessToken}");               
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_url}", payload: payload, headers: header);
                return (payload, response);
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return (payload, new ServiceResponse { StatusMessage = message.StatusMessage, StatusCode = message.StatusCode });
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
                var payload = new
                {
                    Amount = Convert.ToInt32(request.Amount),
                    Reference = request.Reference,
                    PhoneNumber = request.PhoneNumber,
                    CallBackUrl = request.CallBackUrl ?? "http://emmanuelkoech-001-site1.gtempurl.com/banktransfer/callback",
                    ErrorCallBackUrl = request.ErrorCallBackUrl ?? "http://emmanuelkoech-001-site1.gtempurl.com/banktransfer/callback",
                    countryCode = request.CountryCode ?? "KE",
                    telco = request.Telco ?? "SAF"
                };
                var accessToken = await GetAccessToken();
                var header = new Dictionary<string, string>();
                header.Add("Authorization", $"{accessToken?.TokenType} {accessToken?.AccessToken}");
                var pay = JsonConvert.SerializeObject(payload);
                var response = await _httpClient.PostJSONAsync<ServiceResponse>($"{_stkUrl}", payload: payload, headers: header);
                if (response == null || response?.StatusCode != "00")
                    return new ServiceResponse { StatusCode = "01", StatusMessage = response?.StatusMessage ?? "failed" };
                       
                return response;
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseJsonAsync<ServiceResponse>();
                return new ServiceResponse { StatusMessage = JsonConvert.SerializeObject(ex.Message) ?? message?.StatusMessage, StatusCode = message?.StatusCode };
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
