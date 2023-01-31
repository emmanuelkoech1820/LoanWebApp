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
using Apps.Core.Models.OTPModel;
using Apps.Core.Core;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public class OTPController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IBankTransferManager _bankTransferManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        private readonly OTPManager _otpService;

        public OTPController(
            IAccountService accountService,
            IMapper mapper, IBankTransferManager bankTransferManager, IOptions<AppSettings> appSettings, IConfiguration configuration, OTPManager otpService)
        {
            _accountService = accountService;
            _mapper = mapper;
            _bankTransferManager = bankTransferManager;
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _otpService = otpService;
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

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOTP([FromBody] OtpMessage model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _otpService.GenerateOtp(model);
            if (response == null || !response.Successful)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] OtpVerifyMessage model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _otpService.VerifyOTP(model.Reference, model.Operation, model.Source, model.Password);
            if (response == null || !response.Successful)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
        [HttpPost("regenerate")]
        public async Task<IActionResult> ReGenerateOTP(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return BadRequest(ModelState);
            }
            var response = await _otpService.ReGenerateOtp(reference);
            if (response == null || !response.Successful)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }

    }
}
