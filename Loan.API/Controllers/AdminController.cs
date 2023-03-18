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
using WebApi.Consts.Accounts;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public class AdminController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IBankTransferManager _bankTransferManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public AdminController(
            IAccountService accountService,
            IMapper mapper, IBankTransferManager bankTransferManager, IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _accountService = accountService;
            _mapper = mapper;
            _bankTransferManager = bankTransferManager;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }
        public async Task<string> Token(HttpContext context)
        {
            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var tokenHandler = new JwtSecurityTokenHandler();
                var config = _configuration["AppSettings:Secret"];
                var key = Encoding.ASCII.GetBytes(config);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var id = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                return id.ToString();
                //// attach account to context on successful jwt validation
                //context.Items["Account"] = await dataContext.Accounts.FindAsync(accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }


        [HttpGet("loans")]
        public async Task<ServiceResponse<List<LoanAccount>>> GetAllAPlliedLoans(int count = 20)
        {
            var context = HttpContext;
            var referenceId = Token(context).Result;

            var response = await _bankTransferManager.GetAppliedLoans(count);

            if (!response.Successful)
            {
                return new ServiceResponse<List<LoanAccount>>
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,

                };
            }
            return new ServiceResponse<List<LoanAccount>>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = response.ResponseObject

            };


        }
        [HttpGet("requests/{reference}")]
        public async Task<ServiceResponse<LoanAccount>> GetAllRequests(string reference)
        {

            var response = await _bankTransferManager.GetLoanRequest(reference);
            var responses = response.ResponseObject;

            if (!response.Successful)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,

                };
            }
            return new ServiceResponse<LoanAccount>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = responses

            };

        }
        [HttpGet("userdetails/{id}")]
        public async Task<ServiceResponse<AccountResponse>> GetUserById(int id)
        {
            if (id == default(int))
            {
                return new ServiceResponse<AccountResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.INVALID_INPUT_PARAM,

                };
            }
            return await _accountService.GetUserById(id);

        }
        [HttpGet("userdetailslong/{id}")]
        public async Task<ServiceResponse<AdminDetailsResponse>> GetUserByIdLong(int id)
        {
            if (id == default(int))
            {
                return new ServiceResponse<AdminDetailsResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.INVALID_INPUT_PARAM,

                };
            }
            var account = await _accountService.GetUserById(id);
            if (!account.Successful || account.ResponseObject == null)
            {
                return new ServiceResponse<AdminDetailsResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.INVALID_INPUT_PARAM,

                };
            }
            var vehicle = await _bankTransferManager.GetVehicles(id.ToString());
            var loan = await _bankTransferManager.GetLoanRequests(id.ToString());
            return new ServiceResponse<AdminDetailsResponse>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = new AdminDetailsResponse()
                {
                    accountResponse = account?.ResponseObject,
                    LoanAccounts = loan?.ResponseObject,
                    vehicle = vehicle?.ResponseObject
                }

            };

        }

    }
}
