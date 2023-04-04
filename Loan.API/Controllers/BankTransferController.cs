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
using Apps.Core.Models.SMSModels;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public class BankTransferController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IBankTransferManager _bankTransferManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public BankTransferController(
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

        [HttpPost("disburse")]
        
        public async Task<ServiceResponse> InitiateTransaction(BankTransferBinding model)
        {
            var context = HttpContext;
            var user = Token( context);
            var response = await _bankTransferManager.InitiateRequest(model);
            if (!response.Successful)
            {
                return new ServiceResponse
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,

                };
            }
            var responses = response.ResponseObject;

            var validate = await _bankTransferManager.ValidateRequest(responses);
            if (!validate.Successful)
            {
                return new ServiceResponse
                {
                    StatusCode = validate.StatusCode,
                    StatusMessage = validate.StatusMessage,

                };
            }
            return (await Complete(responses, model.LoanStatus));

            //if (!response.Successful)
            //{
            //    return new ServiceResponse
            //    {
            //        StatusCode = response.StatusCode,
            //        StatusMessage = response.StatusMessage,

            //    };
            //}
            //return new ServiceResponse
            //{
            //    StatusCode = ServiceStatusCode.SUCCESSFUL,
            //    StatusMessage = StatusMessage.SUCCESSFUl,

            //};

        }
        [HttpPost("Complete")]
        public async Task<ServiceResponse> Complete(BankTransferRequest model, LoanStatus LoanStatus)
        {
            var context = HttpContext;
            var user = Token(context);
            model.ProfileId = user.Result;
            var complete = await _bankTransferManager.Transfer(model, model.ProfileId, LoanStatus);
            if (complete.StatusCode != "00")
            {
                return new ServiceResponse
                {
                    StatusCode = complete.StatusCode,
                    StatusMessage = complete.StatusMessage,

                };
            }
            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,

            };
        }

        [HttpPost("applyloan")]
        public async Task<ServiceResponse> InitiateLoanTransaction(LoanBindingModel model)
        {
            var context = HttpContext;
            var user = Token(context);
            model.ProfileId = user.Result;
            var response = await _bankTransferManager.InitiateLoanRequest(model);
            var responses = response.ResponseObject;

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
                StatusMessage = StatusMessage.SUCCESSFUl,

            };

        }
        [HttpPost("approveloan")]
        public async Task<ServiceResponse> ApproveLoan(LoanApproval model)
        {
            var context = HttpContext;
            var user = Token(context);
            string profileId = user.Result;
            var response = await _bankTransferManager.ApproveLoanRequest(model, profileId);
            var responses = response.ResponseObject;

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
                StatusMessage = StatusMessage.SUCCESSFUl,

            };

        }
        [HttpGet("getrequests")]
        public async Task<ServiceResponse<List<LoanAccount>>> GetAllRequests()
        {
            var context = HttpContext;
            var referenceId = Token(context).Result;

            var response = await _bankTransferManager.GetLoanRequests(referenceId);
            var responses = response.ResponseObject;

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
                ResponseObject = responses

            };

        }
        [HttpGet("getpendingrequest")]
        public async Task<ServiceResponse<List<LoanAccount>>> GetAllPendingRequests()
        {

            var response = await _bankTransferManager.GetLoanRequests();
            var responses = response.ResponseObject;

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
                ResponseObject = responses

            };

        }
        [HttpPost("addvehicle")]
        public async Task<ServiceResponse<List<LoanAccount>>> AddVehicle(VehicleBindingModel model)
        {
            var context = HttpContext;
            var referenceId = Token(context).Result;
            model.ProfileId = referenceId;
            var response = await _bankTransferManager.AddVehicle(model);

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
                StatusMessage = StatusMessage.SUCCESSFUl
            };

        }
        [HttpGet("getvehicle")]
        public async Task<ServiceResponse<List<Vehicle>>> GetAllvehicles()
        {
            var context = HttpContext;
            var referenceId = Token(context).Result;

            var response = await _bankTransferManager.GetVehicles(referenceId);
            var responses = response.ResponseObject;

            if (!response.Successful)
            {
                return new ServiceResponse<List<Vehicle>>
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,

                };
            }
            return new ServiceResponse<List<Vehicle>>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = responses

            };

        }
        [HttpPost("payloan")]
        public async Task<ServiceResponse> PayLoan(PayLoanBindingModel model)
        {
            var context = HttpContext;
            var referenceId = Token(context).Result;
            var response = await _bankTransferManager.PayLoan(model, referenceId);
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
        [AllowAnonymous]
        [HttpPost("callback")]       
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
        [HttpGet("accounts")]
        public async Task<ServiceResponse<List<TransactingAccount>>> GetTrasactingAccounts()
        {
            var response =  await _bankTransferManager.GetTransactingAccounts();
            return new ServiceResponse<List<TransactingAccount>>
            {
                StatusCode = response.StatusCode,
                StatusMessage = response.StatusMessage,
                ResponseObject = response.ResponseObject
            };
        }
    }
}
