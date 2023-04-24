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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Collections.Specialized.BitVector32;
using System.IO;
using Apps.Core.Models.SMSModels;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public class NyumbaniController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly INyumbaniManager _nyumbaniManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public NyumbaniController(
            IAccountService accountService,
            IMapper mapper, INyumbaniManager nyumbaniManager, IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _accountService = accountService;
            _mapper = mapper;
            _nyumbaniManager = nyumbaniManager;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }
        public async Task<TokenModel> Token(HttpContext context)
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
                return new TokenModel()
                {
                    Id = int.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == "id").Value),
                    ClientId = jwtToken.Claims.FirstOrDefault(x => x.Type.ToLower() == "clientid").Value,
                    Role = jwtToken.Claims.First(x => x.Type.ToLower() == "role").Value,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }

        [HttpPost("add")]

        public async Task<IActionResult> AddProperty(ImagesBindingModel model)
        {
            var context = HttpContext;
            var user = await Token(context);
            model.AgentId = user.Id.ToString();
            model.ClientId = user.ClientId;
            return Ok(await _nyumbaniManager.AddProperty(model));

        }
        [HttpPut("update")]
        public async Task<ServiceResponse<List<LoanAccount>>> UpdatePropertyStatus(string reference, bool status)
        {
            var response = await _nyumbaniManager.UpdateProperty(reference, status);

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
        [HttpGet()]
        public async Task<ServiceResponse<List<PropertyModel>>> GetAllProperties()
        {
            var context = HttpContext;
            var user = Token(context).Result;

            var response = await _nyumbaniManager.GetAllProperty(user.Id.ToString());
            var responses = response.ResponseObject;

            if (!response.Successful)
            {
                return new ServiceResponse<List<PropertyModel>>
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,

                };
            }
            return new ServiceResponse<List<PropertyModel>>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = responses
            };

        }
        [HttpGet("image/{reference}")]
        public async Task<IActionResult> GetImage(string reference)
        {
            var context = HttpContext;
            var user = await Token(context);
            var product = await _nyumbaniManager.FindImageAsync( reference, user.Id.ToString());

            if (product == null || !product.Successful)
            {
                return BadRequest(product);
            }

            //return File(product.Image, "image/jpeg");
            return Ok(product);
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(ImagesBindingModel model)
        {
            var context = HttpContext;
            var user = await Token(context);
            model.ProfileId = user.Id.ToString();
            var product = await _nyumbaniManager.SaveImage(model);
            if (product == null || !product.Successful)
            {
                return BadRequest(product);
            }
            return Ok(product);
        }
    }
}
