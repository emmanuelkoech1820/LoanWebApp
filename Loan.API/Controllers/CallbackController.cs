using Apps.Core.Abstract;
using Apps.Core.Models;
using Apps.Data.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Const;
using WebApi.Consts;
using WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using WebApi.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Core.Helpers;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    [AllowAnonymous]
    public class CallbackController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IBankTransferManager _bankTransferManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public CallbackController(
            IAccountService accountService,
            IMapper mapper, IBankTransferManager bankTransferManager, IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _accountService = accountService;
            _mapper = mapper;
            _bankTransferManager = bankTransferManager;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }
        [HttpPost()]       
        public async Task<ServiceResponse> STKCallBack(STKCallback model)
        {
            var response = await _bankTransferManager.STKCallback(model);
            if (!response.Successful)
            {
                return new ServiceResponse
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,

                };
            }
            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl
            };
        }

    }
}
