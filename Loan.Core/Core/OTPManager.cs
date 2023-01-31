using Apps.Core.Consts;
using Apps.Core.Models.OTPModel;
using Apps.Core.Proxy.Abstract;
using Apps.Data.Entities;
using Apps.Data.Helpers;
using Core.Const;
using Loan.Core.Proxy.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Core.Core
{
    public class OTPManager
    {
        private IConfiguration _configuration;
        private readonly DataContext _context;
        private ITransferProxy _transferProxy;
        private ISmsProxy _smsProxy;
        public OTPManager(IConfiguration configuration, DataContext context, ITransferProxy transferProxy, ISmsProxy smsProxy)
        {
            _configuration = configuration;
            _context = context;
            _transferProxy = transferProxy;
            _smsProxy = smsProxy;
        }
        public async Task<ServiceResponse> GenerateOtp(OtpMessage oMsg)
        {
            var config = GetOtpConfiguration(_configuration, oMsg.Source);
            (var hashedPassword, var password) = GeneratePassword(config.NoOfDigit, $"{oMsg.Reference}{oMsg.Source}{oMsg.Operation}");

            var otp = new Otp()
            {
                CreatedBy = "OtpService",
                Created = DateTime.UtcNow,
                Reference = oMsg.Reference,
                Operation = oMsg.Operation,
                Source = oMsg.Source,
                Platform = oMsg.Platform.ToString(),
                To = oMsg.To,
                NoOfRegenerations = 0,
                RetryAttempts = 0,
                NoOfDigits = config.NoOfDigit,
                ExpiresOn = DateTime.UtcNow.AddMinutes(config.ExpiresInMin), 
                Password = hashedPassword,
                Status = OtpStatus.Generated
            };

            _context.Update(otp);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }
            await _smsProxy.SendSMS(oMsg.To, $"InsureTech registration, Dear customer, please use this digits to complete your registration {password}", "RegisterOTP");

            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = BankTransferAction.INITIATE
            };
        }

        public async Task<ServiceResponse> ReGenerateOtp(string reference)
        {
            var existingOtp = await _context.OTP.FirstOrDefaultAsync(c => c.Reference == reference);

            if (existingOtp == null)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "Sorry, we are unable to generate OTP, please try again later"
                };
            }

            var otpConfig = GetOtpConfiguration(_configuration, existingOtp.Source);

            if (existingOtp.ExpiresOn <= DateTime.UtcNow)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "Sorry, we are unable to generate OTP, please try again later"
                };
            }

            if (existingOtp.NoOfRegenerations >= otpConfig.NumberOfRetries)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "The number of verification codes that can be sent has been exceeded, please try again later"
                };
            }

            (var hashedPassword, var password) = GeneratePassword(existingOtp.NoOfDigits, $"{existingOtp.Reference}{existingOtp.Source}{existingOtp.Operation}");

            ++existingOtp.NoOfRegenerations;
            existingOtp.Password = hashedPassword;
            existingOtp.ExpiresOn = DateTime.UtcNow.AddMinutes(otpConfig.ExpiresInMin); // DateTime.UtcNow.AddMinutes(otpConfig.ExpiresInMin);
            existingOtp.Status = OtpStatus.Regenerated;
            existingOtp.ModifiedBy = "OtpService";
            existingOtp.ModifiedOn = DateTime.UtcNow;
            _context.Update(existingOtp);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }
            await _smsProxy.SendSMS(existingOtp.To, $"InsureTech registration, Dear customer, please use this digits to complete your registration {password}", "RegisterOTP");

            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = BankTransferAction.INITIATE
            };
        }       

        public async Task<ServiceResponse> VerifyOTP(string reference, string operation, string source, string password)
        {
            //var otpRepo = _iuow.Repository<Otp>();
            //var existingOtp = otpRepo.Get(x => x.Reference == reference).SingleOrDefault();
            var otpConfig = GetOtpConfiguration(_configuration, source);

            var existingOtp = await _context.OTP.FirstOrDefaultAsync(c => c.Reference == reference);

            if (existingOtp == null)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "Sorry, we are unable to generate OTP, please try again later"
                };
            }

            if (existingOtp.Operation != operation)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "Operations dont match."
                };
            }

            if (existingOtp.Source != source)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "Sources dont match."
                };
            }                
            

            if (existingOtp.ExpiresOn <= DateTime.UtcNow)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "The verification code you entered has expired or it has already been used, we'll send you another one"
                };
            }

            if (existingOtp.RetryAttempts >= otpConfig.NumberOfRetries)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = "The number of verification codes that can be sent has been exceeded, please try again later"
                };
            }
            var isValid = VerifyPassword(password, $"{existingOtp.Reference}{existingOtp.Source}{existingOtp.Operation}", existingOtp.Password);

            ++existingOtp.RetryAttempts;

            if (isValid)
            {
                existingOtp.ExpiresOn = DateTime.UtcNow;
                existingOtp.UsedOn = DateTime.UtcNow;
                existingOtp.Status = OtpStatus.Success;
            }
            else
            {
                existingOtp.Status = OtpStatus.Invalid;
            }

            existingOtp.ModifiedBy = "OtpService";
            existingOtp.ModifiedOn = DateTime.UtcNow;

            var message = isValid ? "Successful" : "Please ensure that the OTP you entered is correct."; _context.Update(existingOtp);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }
            await _smsProxy.SendSMS(existingOtp.To, $"InsureTech registration, Dear customer, please use this digits to conplete your registration {password}", "RegisterOTP");

            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = message
            };
        }

        public bool VerifySignature(string source, string key, string signature)
        {
            byte[] computedHash;

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(source));
            }
            var base64String = Convert.ToBase64String(computedHash);
            return (signature.Equals(base64String, StringComparison.Ordinal));
        }

        private (byte[], string) GeneratePassword(int length, string key)
        {
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            byte[] computedHash;

            while (password.Length < length)
            {
                password.Append((char)random.Next(48, 58));
            }

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password.ToString()));
            }

            return (computedHash, password.ToString());
        }

        private bool VerifyPassword(string source, string key, byte[] password)
        {
            byte[] computedHash;

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(source));
            }
            var computedHashString = Convert.ToBase64String(computedHash);
            // Verification
            return computedHashString.Equals(Convert.ToBase64String(password), StringComparison.Ordinal);
        }
        private static OtpConfiguration GetOtpConfiguration(IConfiguration configuration, string source)
        {

            var configSection = configuration.GetSection("Otp");
            var config = new OtpConfiguration
            {
                NumberOfRetries = configSection.GetValue<int>("NumberOfRetries"),
                NumberOfRegenerations = configSection.GetValue<int>("NumberOfRegenerations"),
                ExpiresInMin = configSection.GetValue<int>("ExpiresInMin"),
                NotificationTemplate = configSection.GetValue<String>("NotificationTemplate"),
                Institution = configSection.GetValue<String>("Institution"),
                NoOfDigit = configSection.GetValue<int>("NoOfDigit"),
            };

            return config;
        }
    }
}
