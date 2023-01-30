
using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Core.Models.SMSModels;
using Apps.Core.Proxy.Abstract;
using Apps.Core.Utils;
using Apps.Data.Entities;
using Flurl.Http;
using Loan.Core.Proxy.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apps.Core.Proxy
{
    public class SmsService : ISmsProxy
    {
        private IConfiguration _configuration;
        private HttpClientUtil _httpClient;
        private string _smsUrl;
        private string _apikey;
        private string _partnerId;
        private string _ShortCode;
        private string _passType;
        private readonly IHttpContextAccessor _httpAccessor;
        public SmsService(IHttpContextAccessor httpAccessor, IConfiguration configuration, HttpClientUtil httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpAccessor = httpAccessor;
            _smsUrl = $"{configuration["SMSConfig:url"]}";
            _apikey = $"{configuration["SMSConfig:apikey"]}";
            _partnerId = $"{configuration["SMSConfig:partnerID"]}";
            _ShortCode = $"{configuration["SMSConfig:shortcode"]}";
            _passType = $"{configuration["SMSConfig:pass_type"]}";
        }


        public async Task<ServiceResponse> SendSMS(string phoneNumber, string message, string operation)
        {
            var sendSms = _configuration[$"SMSConfig:Operation:{operation}"];
            if(!bool.Parse(sendSms))
            {
                return new ServiceResponse()
                {
                    StatusCode = "00",
                    StatusMessage = "Request received, sms not sent"
                };
            }
            var request = new SMSRequestModel()
            {
                apikey = _apikey,
                message = message,
                shortcode = _ShortCode,
                mobile = phoneNumber,
                partnerID = _partnerId,
                pass_type = _passType
            };
            var pay = JsonConvert.SerializeObject(request);
            try
            {
                var response = await _httpClient.PostJSONAsync<SMSResponseModel>($"{_smsUrl}", payload: request);
                return new ServiceResponse()
                {
                    StatusCode = "00",
                    StatusMessage = "Request received, sms sent"
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
