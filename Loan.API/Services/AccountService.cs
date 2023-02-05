using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Consts.Accounts;
using WebApi.Consts;
using WebApi.Const;
using WebApi.Helpers;
using Apps.Data.Entities;
using CApps.Dataore.Entities;
using Apps.Data.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Apps.Core.Core;
using Apps.Core.Models.OTPModel;
using System.Threading.Tasks;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Loan.Core.Proxy.Abstract;

namespace WebApi.Services
{
    public interface IAccountService
    {
        ServiceResponse<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        Task<ServiceResponse<OtpMessage>> Register(RegisterRequest model, string origin);
        ServiceResponse<ValidateResetTokenRequest> VerifyEmail(string token);
        ServiceResponse<ValidateTokenResponseModel> ForgotPassword(ForgotPasswordRequest model, string origin);
        bool ValidateResetToken(ResetPasswordRequest model);
        ServiceResponse ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(int id);
        AccountResponse Create(CreateRequest model);
        AccountResponse Update(int id, UpdateRequest model);
        void Delete(int id);
        Task<ServiceResponse> VerifyOtp(VerifyPhoneNumberModel model);
        Task<ServiceResponse<AccountResponse>> GetUserByPhone(string phoneNumber);
    }

    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly OTPManager _otpService;

        public AccountService(
            DataContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService,
            OTPManager otpService)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _otpService = otpService;
        }

        public ServiceResponse<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email || x.PhoneNumber == model.PhoneNumber);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PasswordHash))
            {
                return new ServiceResponse<AuthenticateResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.PASSWORD_VALIDATION_FAILED,

                };
            }
            if (account.ClientId == null)
            {
                return new ServiceResponse<AuthenticateResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.PASSWORD_VALIDATION_FAILED,

                };
            }


            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(account);
            var refreshToken = generateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return new ServiceResponse<AuthenticateResponse>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = response
            };
        }
        public ServiceResponse Connected(AuthenticateRequest model, string ipAddress)
        {

            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.CNNECTION_TEST,

            };

        }
        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            removeOldRefreshTokens(account);

            _context.Update(account);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(account);
            _context.SaveChanges();
        }
        private string GetUser(string clientId)
        {
            var user = _appSettings.ClientId.FirstOrDefault(x => x.Key == clientId).Value;
            return user;
        }

        public async Task<ServiceResponse<OtpMessage>> Register(RegisterRequest model, string origin)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email || x.PhoneNumber == model.PhoneNumber))
            {
                // send already registered error in email to prevent account enumeration
                //sendAlreadyRegisteredEmail(model.Email, origin);
                return (new ServiceResponse<OtpMessage>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.USER_NOT_FOUND

                });
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            // first registered account is an admin
            var isFirstAccount = _context.Accounts.Count() == 0;
            if (isFirstAccount)
                account.Role = Role.Admin;

            if (!string.IsNullOrEmpty(model.ClientId))
            {
                account.Role = (Role)Enum.Parse(typeof(Role), (GetUser(model.ClientId)));
            }
            else
            {
                return (new ServiceResponse<OtpMessage>
                {
                    StatusCode = ServiceStatusCode.TRANSACTION_FAILED,
                    StatusMessage = StatusMessage.INVALID_CLIENT_ID

                });
            }
            //account.Created = DateTime.UtcNow;
            //account.VerificationToken = randomTokenString();
            //account.Verified = DateTime.UtcNow;
            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);
            var otpPayload = new Apps.Core.Models.OTPModel.OtpMessage()
            {
                Operation = "RegisterOTP",
                Source = "Android",
                To = account.PhoneNumber,
                Reference = new Random().Next(99999999).ToString()
            };

            // save account
            _context.Accounts.Add(account);
            var response = _context.SaveChanges();
            if (response == 1)
            {
                await _otpService.GenerateOtp(otpPayload);
                return (new ServiceResponse<OtpMessage>
                {
                    StatusCode = ServiceStatusCode.SUCCESSFUL,
                    StatusMessage = StatusMessage.REGISTER_SUCCESS,
                    ResponseObject = otpPayload

                }); ;
            }
            return (new ServiceResponse<OtpMessage>
            {
                StatusCode = ServiceStatusCode.INVALID_REQUEST,
                StatusMessage = StatusMessage.USER_NOT_FOUND

            });


            // send email
            // sendVerificationEmail(account, origin);           
        }

        public ServiceResponse<ValidateResetTokenRequest> VerifyEmail(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null)
                return (new Consts.ServiceResponse<ValidateResetTokenRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.USER_NOT_FOUND

                });

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
            return (new Consts.ServiceResponse<ValidateResetTokenRequest>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.REGISTER_SUCCESS

            });
        }

        public ServiceResponse<ValidateTokenResponseModel> ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email || x.PhoneNumber == model.PhoneNumber);

            // always return ok response to prevent email enumeration
            if (account == null)
            {
                return (new Consts.ServiceResponse<ValidateTokenResponseModel>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.USER_NOT_FOUND

                });
            }

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            var response = _context.SaveChanges();
            if (response == 1)
            {
                return (new Consts.ServiceResponse<ValidateTokenResponseModel>
                {
                    StatusCode = ServiceStatusCode.SUCCESSFUL,
                    StatusMessage = StatusMessage.REGISTER_SUCCESS,
                    ResponseObject = new ValidateTokenResponseModel { Token = account.ResetToken, ExpiresIn = account.ResetTokenExpires }

                });

            }

            return (new Consts.ServiceResponse<ValidateTokenResponseModel>
            {
                StatusCode = ServiceStatusCode.INVALID_REQUEST,
                StatusMessage = StatusMessage.COULD_NOT_SEND_RESET_Token

            });
            // send email
            // sendPasswordResetEmail(account, origin);
        }

        public bool ValidateResetToken(ResetPasswordRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);
            if (account == null)
            {
                return false;
            }
            return true;



        }

        public ServiceResponse ResetPassword(ResetPasswordRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
            {
                return (new Consts.ServiceResponse<ValidateTokenResponseModel>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.USER_NOT_FOUND

                });
            }

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            var response = _context.SaveChanges();
            if (response == 1)
            {
                return (new Consts.ServiceResponse
                {
                    StatusCode = ServiceStatusCode.SUCCESSFUL,
                    StatusMessage = StatusMessage.SUCCESSFUl
                });
            }
            return (new Consts.ServiceResponse
            {
                StatusCode = ServiceStatusCode.INVALID_REQUEST,
                StatusMessage = StatusMessage.PASSWORD_RESET_FAILED
            });

        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _context.Accounts;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(int id)
        {
            var account = getAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(int id, UpdateRequest model)
        {
            var account = getAccount(id);

            // validate
            if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BC.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(int id)
        {
            var account = getAccount(id);
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        // helper methods

        private Account getAccount(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private (RefreshToken, Account) getRefreshToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken, account);
        }

        private string generateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()), new Claim("NationalId", account.IdNumber.ToString()), new Claim("id", account.Id.ToString()), new Claim("ClientId", account.ClientId.ToString()), new Claim("Role", GetUser(account.ClientId.ToString())) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void sendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }

        public async Task<ServiceResponse> VerifyOtp(VerifyPhoneNumberModel model)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.PhoneNumber == model.PhoneNumber);

            if (account == null || account.IsVerified)
            {
                return new ServiceResponse<AuthenticateResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.PASSWORD_VALIDATION_FAILED,

                };
            }
            var verify =  await _otpService.VerifyOTP(model.Reference, "RegisterOTP", "Android", model.Otp);
            if(verify == null || !verify.Successful)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.PASSWORD_VALIDATION_FAILED,
                };
            }

            account.VerificationToken = randomTokenString();
            account.Verified = DateTime.UtcNow;
            _context.Update(account);
            _context.SaveChanges();

           // await _smsProxy.SendSMS(existingOtp.To, $"InsureTech registration, Dear customer, your registration is ");
            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
            };
        }

        public async Task<ServiceResponse<AccountResponse>> GetUserByPhone(string phoneNumber)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.PhoneNumber == phoneNumber);

            if (account == null || account.IsVerified)
            {
                return new ServiceResponse<AccountResponse>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.PASSWORD_VALIDATION_FAILED,

                };
            }
            return new ServiceResponse<AccountResponse>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = new AccountResponse
                {
                    Email = account.Email,
                    FirstName  = account.FirstName,
                    LastName = account.LastName,
                    Id = account.Id,
                    IsVerified = account.IsVerified,
                    Role = account.Role.ToString(),
                    Title = account.Title,
                    Created = account.Created,
                    Updated = account.Updated
                        
                }
            };
        }
    }
}
