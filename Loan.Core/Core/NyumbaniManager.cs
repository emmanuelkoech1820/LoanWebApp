
using Apps.Core.Abstract;
using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Core.Proxy.Abstract;
using Apps.Core.Utils;
using Apps.Data.Entities;
using Apps.Data.Helpers;
using Core.Const;
using Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apps.Core.Core
{
    public class NyumbaniManager : INyumbaniManager
    {
        private IConfiguration _configuration;
        private HttpClientUtil _httpClient;
        private string _baseUrl;
        private string _url;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly DataContext _context;
        private ITransferProxy _transferProxy;
        public NyumbaniManager(IHttpContextAccessor httpAccessor, IConfiguration configuration, HttpClientUtil httpClient, DataContext context, ITransferProxy transferProxy)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _url = $"{configuration["Proxy:BankTransfer"]}/intra";
            _httpAccessor = httpAccessor;
            _context = context;
            _transferProxy  = transferProxy;
        }
       
        public async Task<ServiceResponse<List<PropertyModel>>> GetAllProperty(string agentId)
        {
            var result = await _context.Property.Where(c => c.AgentId == agentId).ToListAsync();
            if (result == null)
            {
                return new ServiceResponse<List<PropertyModel>>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            };
            return new ServiceResponse<List<PropertyModel>>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }

        public async Task<ServiceResponse> AddProperty(PropertyBindingModel model)
        {
            if (string.IsNullOrEmpty(model?.AgentId.ToString()))
            {
                throw new ArgumentNullException(nameof(model.AgentId));
            }
            var request = new PropertyModel()
            {
                Reference = new Guid().ToString(),
                AgentId = model.AgentId.ToString(),
                Bathrooms = model.Bathrooms,
                AdditionalInformation = model.AdditionalInformation,
                Bedrooms = model.Bedrooms,
                Kitchens = model.Kitchens,
                LocationId = model.LocationId,
                Price = model.Price,
                PropertyType = model.PropertyType,
                IsEnabled = true
            };
            _context.Update(request);
            var result =  _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.FAILED
                };
            }

            return new ServiceResponse<BankTransferRequest>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl
            };

        }
        public async Task<ServiceResponse> UpdateProperty(string reference, bool status)
        {
            var result = await _context.Property.FirstOrDefaultAsync(c => c.Reference == reference);
            if (result == null)
            {
                return new ServiceResponse<List<PropertyModel>>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            };
            result.IsEnabled = status;
            _context.Update(result);
            var response = _context.SaveChanges();
            if (response < 1)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.FAILED
                };
            }

            return new ServiceResponse<BankTransferRequest>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl
            };

        }
    }
}
