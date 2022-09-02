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
using System.Net;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BankTransferController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IBankTransferManager _bankTransferManager;
        private readonly IMapper _mapper;

        public BankTransferController(
            IAccountService accountService,
            IMapper mapper, IBankTransferManager bankTransferManager)
        {
            _accountService = accountService;
            _mapper = mapper;
            _bankTransferManager = bankTransferManager;
        }

        [HttpPost("disburse")]
        public async Task<ServiceResponse> InitiateTransaction(BankTransferBinding model)
        {
           
            var response = await _bankTransferManager.InitiateRequest(model);
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
            return (await Complete(responses));

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
        public async Task<ServiceResponse> Complete(BankTransferRequest model)
        {
            var complete = await _bankTransferManager.Transfer(model);
            if (!complete.Successful)
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
            var a = CurrentUser;
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

            var response = await _bankTransferManager.ApproveLoanRequest(model);
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
        [HttpPost("getrequests")]
        public async Task<ServiceResponse<List<LoanAccount>>> GetAllRequests(string reference, int statusKey)
        {

            var response = await _bankTransferManager.GetLoanRequests(reference, statusKey);
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

    }
}
