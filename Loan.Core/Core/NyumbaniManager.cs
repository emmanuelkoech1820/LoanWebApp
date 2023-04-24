
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            _transferProxy = transferProxy;
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

        public async Task<ServiceResponse<object>> AddProperty(ImagesBindingModel model)
        {
            if (string.IsNullOrEmpty(model?.AgentId.ToString()))
            {
                throw new ArgumentNullException(nameof(model.AgentId));
            }
            var im = new List<Images>();
           
            var request = new PropertyModel()
            {
                Reference = Guid.NewGuid().ToString(),
                AgentId = model.AgentId.ToString(),
                Bathrooms = model.Bathrooms,
                AdditionalInformation = model.AdditionalInformation,
                Bedrooms = model.Bedrooms,
                Kitchens = model.Kitchens,
                LocationId = model.LocationId,
                Price = model.Price,
                PropertyType = model.PropertyType,
                IsEnabled = true,
                Images = im
            };
            foreach (var item in model.Image)
            {
                im.Add(new Images()
                {
                    Description = item.Description,
                    Image = item.Image,
                    Reference = request.Reference,
                    ProfileId = model.AgentId

                });

            }
            _context.Update(request);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<object>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.FAILED
                };
            }

            return new ServiceResponse<object>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = new { Reference = request.Reference }
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

        public async Task<ServiceResponse<ImagesBindingModel>> FindImageAsync(string reference, string profileId)
        {
            var product = await _context.Property.Include(c => c.Images).FirstOrDefaultAsync(c => c.Reference == reference);
            if (product == null)
            {
                return new ServiceResponse<ImagesBindingModel>
                {
                    StatusCode = ServiceStatusCode.DUPLICATE_REQUEST,
                    StatusMessage = StatusMessage.DUPLICATE_REQUEST
                };
            }
            var im = new List<Images>();
            foreach(var ima in product.Images)
            {
                im.Add(ima);
            }
            var response = new ImagesBindingModel()
            {
                AdditionalInformation = product.AdditionalInformation,
                AgentId = product.AgentId,
                Bathrooms = product.Bathrooms,
                Bedrooms = product.Bedrooms,
                Description = product.Images.FirstOrDefault()?.Description,
                Image = im

            };
            return new ServiceResponse<ImagesBindingModel>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = response
            };

        }

        public async Task<ServiceResponse> SaveImage(ImagesBindingModel model)
        {

            var product = await _context.Images.FirstOrDefaultAsync(c => c.Reference == model.Reference);
            if (product != null)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.DUPLICATE_REQUEST,
                    StatusMessage = StatusMessage.DUPLICATE_REQUEST
                };
            }
            //using (var memoryStream = new MemoryStream())
            //{
            //    await model.Image.CopyToAsync(memoryStream);
            //    product.Image = memoryStream.ToArray();
            //}
            product = new Images()
            {
               // Image = model.Image,
                Reference = model.Reference,
                ProfileId = model.ProfileId,
                Description = model.Description
            };
            _context.Update(product);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse
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
