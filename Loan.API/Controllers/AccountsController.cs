﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi.Const;
using WebApi.Consts;
using WebApi.Consts.Accounts;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountsController(
            IAccountService accountService,
            IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public ActionResult<ServiceResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.PhoneNumber))
            {
                return new ServiceResponse<AuthenticateResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.EMAIL_PHONE_REQUIRED,

                };
            }
            var response = _accountService.Authenticate(model, ipAddress());
            if (response.Successful)
            {
                setTokenCookie(response.ResponseObject.RefreshToken);
            }
            return Ok(response);

        }        
        [HttpGet("connected")]
        public ActionResult<ServiceResponse> Connected(AuthenticateRequest model)
        {
            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.CNNECTION_TEST,

            };
        }

        [HttpPost("refresh-token")]
        public ActionResult<AuthenticateResponse> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _accountService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        //[Authorize]
        //[HttpPost("revoke-token")]
        //public IActionResult RevokeToken(RevokeTokenRequest model)
        //{
        //    // accept token from request body or cookie
        //    var token = model.Token ?? Request.Cookies["refreshToken"];

        //    if (string.IsNullOrEmpty(token))
        //        return BadRequest(new { message = "Token is required" });

        //    // users can revoke their own tokens and admins can revoke any tokens
        //    if (!Account.OwnsToken(token) && Account.Role != Role.Admin)
        //        return Unauthorized(new { message = "Unauthorized" });

        //    _accountService.RevokeToken(token, ipAddress());
        //    return Ok(new { message = "Token revoked" });
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            var response = await _accountService.Register(model, Request.Headers["origin"]);
            if (response != null && response.StatusCode == ServiceStatusCode.SUCCESSFUL)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok("Loan App is running");
        }

        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequest model)
        {
            _accountService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [HttpPost("verifyphone")]
        public async Task<ServiceResponse> VerifyEmail(VerifyPhoneNumberModel model)
        {
            return await _accountService.VerifyOtp(model);
        }

        [HttpPost("forgot-password")]
        public ActionResult<ServiceResponse<ValidateTokenResponseModel>> ForgotPassword(ForgotPasswordRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.PhoneNumber))
            {
                return new ServiceResponse<ValidateTokenResponseModel>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.EMAIL_PHONE_REQUIRED,

                };
            }
            var response = _accountService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(response);
        }

        [HttpPost("validate-reset-token")]
        public IActionResult ValidateResetToken(ResetPasswordRequest model)
        {
            _accountService.ValidateResetToken(model);
            return Ok(new { message = "Token is valid" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            var validationResult = _accountService.ValidateResetToken(model);
            if (!validationResult)
            {
                return BadRequest(new Consts.ServiceResponse<AuthenticateResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.RESET_TOKEN_INVALID

                });
            }
            var resetResponse = _accountService.ResetPassword(model);
            return Ok(resetResponse);
        }
        [HttpPost("details")]
        public async Task<ServiceResponse<AccountResponse>> GetUserByPhoneNumber(string phoneNumber)
        {
            if(phoneNumber == null)
            {
                return new ServiceResponse<AccountResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.EMAIL_PHONE_REQUIRED,

                };
            }
            return await _accountService.GetUserByPhone(phoneNumber);
               
        }

        //[Authorize(Role.Admin)]
        //[HttpGet]
        //public ActionResult<IEnumerable<AccountResponse>> GetAll()
        //{
        //    var accounts = _accountService.GetAll();
        //    return Ok(accounts);
        //}

        //[Authorize]
        //[HttpGet("{id:int}")]
        //public ActionResult<AccountResponse> GetById(int id)
        //{
        //    // users can get their own account and admins can get any account
        //    if (id != Account.Id && Account.Role != Role.Admin)
        //        return Unauthorized(new { message = "Unauthorized" });

        //    var account = _accountService.GetById(id);
        //    return Ok(account);
        //}

        //[Authorize(Role.Admin)]
        //[HttpPost]
        //public ActionResult<AccountResponse> Create(CreateRequest model)
        //{
        //    var account = _accountService.Create(model);
        //    return Ok(account);
        //}

        //[Authorize]
        //[HttpPut("{id:int}")]
        //public ActionResult<AccountResponse> Update(int id, UpdateRequest model)
        //{
        //    // users can update their own account and admins can update any account
        //    if (id != Account.Id && Account.Role != Role.Admin)
        //        return Unauthorized(new { message = "Unauthorized" });

        //    // only admins can update role
        //    if (Account.Role != Role.Admin)
        //        model.Role = null;

        //    var account = _accountService.Update(id, model);
        //    return Ok(account);
        //}

        //[Authorize]
        //[HttpDelete("{id:int}")]
        //public IActionResult Delete(int id)
        //{
        //    // users can delete their own account and admins can delete any account
        //    if (id != Account.Id && Account.Role != Role.Admin)
        //        return Unauthorized(new { message = "Unauthorized" });

        //    _accountService.Delete(id);
        //    return Ok(new { message = "Account deleted successfully" });
        //}

        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
