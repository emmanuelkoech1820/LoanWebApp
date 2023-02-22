
using Apps.Core.Abstract;
using Apps.Core.Consts;
using Apps.Core.Models;
using Apps.Core.Proxy.Abstract;
using Apps.Core.Utils;
using Apps.Data.Entities;
using Apps.Data.Helpers;
using Core.Const;
using Core.Helpers;
using Loan.Core.Proxy.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Apps.Core.Core
{
    public class BankTransferManager : IBankTransferManager
    {
        private IConfiguration _configuration;
        private HttpClientUtil _httpClient;
        private string _baseUrl;
        private string _url;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly DataContext _context;
        private ITransferProxy _transferProxy;
        private ISmsProxy _smsProxy;
        public BankTransferManager(IHttpContextAccessor httpAccessor, IConfiguration configuration, HttpClientUtil httpClient, DataContext context, ITransferProxy transferProxy, ISmsProxy smsProxy)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _url = $"{configuration["Proxy:BankTransfer"]}/intra";
            _httpAccessor = httpAccessor;
            _context = context;
            _transferProxy = transferProxy;
            _smsProxy = smsProxy;
        }
        public async Task<ServiceResponse<BankTransferRequest>> GetBankTransferRequest(string reference)
        {
            var result = await _context.BankTransferRequest.Include(c => c.Histories).FirstOrDefaultAsync(c => c.Reference == reference);
            if (result == null)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            }
            return new ServiceResponse<BankTransferRequest>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }
        public async Task<ServiceResponse<LoanAccount>> GetLoanRequest(string reference)
        {
            var result = await _context.LoanAccount.FirstOrDefaultAsync(c => c.Reference == reference);
            if (result == null)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            }
            return new ServiceResponse<LoanAccount>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }
        public async Task<ServiceResponse<LoanRepayment>> GetLoanRepaymentRequest(string reference)
        {
            var result = await _context.loanRepayment.FirstOrDefaultAsync(c => c.Reference == reference);
            if (result == null)
            {
                return new ServiceResponse<LoanRepayment>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            }
            return new ServiceResponse<LoanRepayment>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }
        public async Task<ServiceResponse<List<LoanAccount>>> GetLoanRequests(string reference)
        {
            //var result = string.IsNullOrWhiteSpace(reference) ? await _context.LoanAccount.Where(c=> c.LoanAprroved == (DisbursmentStatus)statusKey).ToListAsync() :
            //                                                    await _context.LoanAccount.Where(c=> c.Reference == reference).ToListAsync();
            //if (result.Count < 1 || result == null)
            //{
            //    return new ServiceResponse<List<LoanAccount>>
            //    {
            //        StatusCode = ServiceStatusCode.INVALID_REQUEST,
            //        StatusMessage = StatusMessage.REQUEST_NOT_FOUND
            //    };
            //}
            var result = await _context.LoanAccount.Where(c => c.ProfileId == reference).ToListAsync();
            if (result == null)
            {
                return new ServiceResponse<List<LoanAccount>>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            };
            return new ServiceResponse<List<LoanAccount>>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }
        public async Task<ServiceResponse<List<LoanAccount>>> GetLoanRequests()
        {

            var result = await _context.LoanAccount.Where(c => !c.LoanAprroved).ToListAsync();
            if (result == null)
            {
                return new ServiceResponse<List<LoanAccount>>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            };
            return new ServiceResponse<List<LoanAccount>>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }

        public async Task<ServiceResponse> ValidateRequest(BankTransferRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(BankTransferRequest));
            }
            if (request.Status != BankTransferStatus.PENDING_VALIDATION)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.VALIDATION_RULE_BROKEN
                };
            }
            //add any other custom validation
            request.Histories.Add(new History
            {
                Action = BankTransferAction.VALIDATE_REQUEST,
                Description = "Validate bank transfer request."
            });

            request.Status = BankTransferStatus.PENDING_DEBIT;
            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.REQUEST_VALID
            };

        }

        public async Task<ServiceResponse> Transfer(BankTransferRequest request, string profileId)
        {
            if (request == null || request.Status != BankTransferStatus.PENDING_DEBIT)
            {
                throw new ArgumentNullException(nameof(BankTransferRequest));
            }
            request.Status = BankTransferStatus.PENDING_Transfer;
            var result = new ServiceResponse();
            var loanRequest = await GetLoanRequest(request.LoanRequestId);
            object payld = new object();
            if (request.TransferType.ToLower() == "intrabank")
            {
                (payld, result) = await _transferProxy.Intrabank(request);
            }
            else
            {
                (payld, result) = await _transferProxy.Interbank(request);

            }
            request.Histories.Add(new History
            {
                Action = "Transfer result",
                Description = $"JsonResult: {JsonConvert.SerializeObject(payld)}  {JsonConvert.SerializeObject(result)}"
            });
            _context.Update(loanRequest.ResponseObject);
            _context.SaveChanges();
            if (result == null || !result.Successful)
            {

                request.Status = BankTransferStatus.FAILED;
                return new ServiceResponse
                {
                    StatusCode = result?.StatusCode ?? ServiceStatusCode.TRANSACTION_FAILED,
                    StatusMessage = result?.StatusMessage ?? StatusMessage.TRANSFER_FAILED
                };

            }
            request.Histories.Add(new History
            {
                Action = BankTransferAction.TRANSFER_SUCCESS,
                Description = "Transfer Successful"
            });
            await _smsProxy.SendSMS("", $"Confirmed, Your loan is repayment of Ksh {loanRequest.ResponseObject.DisbursedAmount} is received, Loan balance is {loanRequest.ResponseObject.DisbursedAmount} as at {DateTime.Now}", "loanDisbursed");
            loanRequest.ResponseObject.DisbursmentStatus = DisbursmentStatus.Disbursed;
            loanRequest.ResponseObject.LoanHistories = new List<LoanHistory>()
            {
                new LoanHistory
                {
                    Action = "Disbursed",
                    BorrowedAmount = request.Amount,
                    Description = "Loan Approved",
                    PerformedBy = $"Loan approved by {profileId}"
                }
            };
            _context.Update(loanRequest.ResponseObject);
            var results = _context.SaveChanges();
            if (results < 1)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }
            request.Status = BankTransferStatus.SUCCESSFUL;
            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl
            };
        }

        public Task UpdateRequest(BankTransferRequest request)
        {
            throw new NotImplementedException();
        }

        public string GenerateSignature(BankTransferRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<BankTransferRequest>> InitiateRequest(BankTransferBinding model)
        {
            if (string.IsNullOrEmpty(model?.Reference))
            {
                throw new ArgumentNullException(nameof(model.Reference));
            }
            var request = await GetBankTransferRequest(model.Reference);
            var loanRequest = await GetLoanRequest(model.LoanReference);
            if (loanRequest.ResponseObject == null || !loanRequest.ResponseObject.LoanAprroved)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.UNAPPROVED_REQUEST
                };
            }

            if (request.ResponseObject != null)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.DUPLICATE_REQUEST,
                    StatusMessage = StatusMessage.DUPLICATE_REQUEST
                };
            }
            request.ResponseObject = new BankTransferRequest()
            {
                Amount = model.Amount,
                BankId = model.BankId,
                Currency = model.Currency,
                DestinationAccount = model.DestinationAccount,
                DestinationBankCode = model.DestinationBankCode,
                DestinationName = model.DestinationName,
                Narration = model.Narration,
                PaymentReason = model.PaymentReason,
                SourceAccount = model.SourceAccount,
                Status = BankTransferStatus.PENDING_VALIDATION,
                Reference = model.Reference,
                LoanRequestId = model.LoanReference,
                TransferType = model.TransferType,
                Histories = new List<History>
                {
                    new History
                    {
                        Description = "Initiate",
                        Action = "Initiate",
                        PerformedBy = "customer"
                    }
                }
            };
            _context.Update(request.ResponseObject);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<BankTransferRequest>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }

            return new ServiceResponse<BankTransferRequest>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = BankTransferAction.INITIATE,
                ResponseObject = request.ResponseObject
            };

        }

        public async Task<ServiceResponse<LoanAccount>> InitiateLoanRequest(LoanBindingModel model)
        {
            if (string.IsNullOrEmpty(model?.Reference))
            {
                throw new ArgumentNullException(nameof(model.Reference));
            }
            var request = await GetLoanRequest(model.Reference);
            if (request.ResponseObject != null)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = ServiceStatusCode.DUPLICATE_REQUEST,
                    StatusMessage = StatusMessage.DUPLICATE_REQUEST
                };
            }
            request.ResponseObject = new LoanAccount()
            {
                RequestedAmount = model.Amount,
                Currency = model.Currency,
                DestinationAccount = model.DestinationAccount,
                DestinationName = model.DestinationName,
                LoanReason = model.LoanReason,
                Reference = model.Reference,
                DestinationBankCode = model.DestinationBankCode,
                RepaymentPeriod = model.RepaymentPeriod,
                ProfileId = model.ProfileId,
                LoanHistories = new List<LoanHistory>
                {
                    new LoanHistory
                    {
                        Description = "Customer Borrow Money",
                        Action = "Customer Initiate",
                        PerformedBy = "customer"
                    }
                },
                VehicleReferenceNumber = model.VehicleReferenceNumber,
                VehicleRegistrationNumber = model.VehicleRegistrationNumber
            };

            _context.Update(request.ResponseObject);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }

            return new ServiceResponse<LoanAccount>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = BankTransferAction.INITIATE,
                ResponseObject = request.ResponseObject
            };
        }

        public async Task<ServiceResponse<LoanAccount>> ApproveLoanRequest(LoanApproval model, string profileId)
        {
            var loanRequest = await GetLoanRequest(model.Reference);
            if (loanRequest.ResponseObject == null)
            //|| loanRequest.ResponseObject.LoanAprroved != LoanApprovalStatus.Approved)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.UNAPPROVED_REQUEST
                };
            }
            loanRequest.ResponseObject.DisbursedAmount = model.Amount;
            loanRequest.ResponseObject.DisbursmentStatus = model.Status;
            loanRequest.ResponseObject.DestinationAccount = model.DestinationAccount;
            loanRequest.ResponseObject.LoanAprroved = model.LoanApprovalStatus;
            loanRequest.ResponseObject.LoanHistories = new List<LoanHistory>()
            {
                new LoanHistory
                {
                    Action = "Approval",
                    BorrowedAmount = model.Amount,
                    Description = "Loan Approved",
                    PerformedBy = $"Loan approved by {profileId}"
                }
            };


            _context.Update(loanRequest.ResponseObject);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse<LoanAccount>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }
            return new ServiceResponse<LoanAccount>
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = BankTransferAction.INITIATE,
                ResponseObject = loanRequest.ResponseObject
            };
        }

        public async Task<ServiceResponse> AddVehicle(VehicleBindingModel request)
        {
            var vehicle = new Vehicle()
            {
                InsturanceStartDate = request.InsturanceStartDate,
                InsuranceCoverType = request.InsuranceCoverType,
                Reference = request.Reference,
                RegistrationNumber = request.RegistrationNumber,
                VehicleCategory = request.VehicleCategory,
                VehicleModel = request.VehicleModel,
                VehicleType = request.VehicleType,
                VehicleValue = request.VehicleValue,
                YearOfManufacture = request.YearOfManufacture,
                ProfileId = request.ProfileId
            };

            _context.Update(vehicle);
            var result = _context.SaveChanges();
            if (result < 1)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.TRANSFER_FAILED
                };
            }

            return new ServiceResponse
            {
                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = BankTransferAction.INITIATE
            };
        }

        public async Task<ServiceResponse<List<Vehicle>>> GetVehicles(string profileId)
        {
            var result = await _context.Vehicles.Where(c => c.ProfileId == profileId).ToListAsync();
            if (result == null)
            {
                return new ServiceResponse<List<Vehicle>>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            };
            return new ServiceResponse<List<Vehicle>>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }

        public async Task<ServiceResponse> PayLoan(PayLoanBindingModel request, string profileId)
        {
            var loanRepaymentRequest = await GetLoanRepaymentRequest(request.Reference);
            if (loanRepaymentRequest.ResponseObject != null)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.DUPLICATE_REQUEST
                };

            }
            var loanRequest = await GetLoanRequest(request.LoanReference);
            if (loanRequest.ResponseObject == null)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.VALIDATION_RULE_BROKEN
                };

            }
            var LoanRepayment = loanRepaymentRequest.ResponseObject;
             LoanRepayment = new LoanRepayment()
            {
                Amount = request.Amount,
                ProfileId = profileId,
                Currency = "KES",
                Status = RepaymentStatus.STKPushReceived,
                SourcePhoneNumber = request.PhoneNumber,
                Reference = request.Reference,
                JsonRequest = JsonConvert.SerializeObject(request)
            };
            _context.Update(LoanRepayment);
            _context.SaveChanges();
            var response = await _transferProxy.PayLoan(request);
            if(response == null || response.StatusCode != "00")
            {
                LoanRepayment.Status = RepaymentStatus.STKPushSent;
                LoanRepayment.JsonResponse = JsonConvert.SerializeObject(response);
                _context.Update(LoanRepayment);
                _context.SaveChanges();
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.FAILED
                };
            }
            LoanRepayment.Status = RepaymentStatus.STKPushSent;
            LoanRepayment.JsonResponse = JsonConvert.SerializeObject(response);
            loanRequest.ResponseObject.RepaymentStatus = "REPAID";
            loanRequest.ResponseObject.RepaidAmount = request.Amount;
            _context.Update(LoanRepayment);
            _context.SaveChanges();
            return new ServiceResponse()
            { StatusCode = "00", StatusMessage = "Success" };

        }

        public async Task<ServiceResponse> STKCallback(STKCallback model)
        {

            var loanRepayment = await _context.loanRepayment.FirstOrDefaultAsync(c => c.Reference == model.Reference);
            var request = await GetLoanRequest(loanRepayment.Reference);
            if (request.ResponseObject == null)
            {
                return new ServiceResponse
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.VALIDATION_RULE_BROKEN
                };
            }
            var loanRequest = request.ResponseObject;
            loanRequest.RepaidAmount = loanRepayment.Amount;
            loanRequest.LoanBalance = loanRequest.DisbursedAmount - loanRepayment.Amount;
            _context.Update(loanRequest);
            _context.SaveChanges();

            await _smsProxy.SendSMS(loanRepayment.SourcePhoneNumber, $"Confirmed, Your loan repayment of Ksh {loanRepayment.Amount} is received, Loan balance is {loanRequest.LoanBalance} as at {DateTime.Now}", "loanRepaid");

            return new ServiceResponse()
            { StatusCode = "00", StatusMessage = "Success" };
        }

        public async Task<ServiceResponse<List<LoanAccount>>> GetAppliedLoans(int count)
        {
            var result = await _context.LoanAccount.Where(c => !c.LoanAprroved).Take(count).ToListAsync();
            if (result == null)
            {
                return new ServiceResponse<List<LoanAccount>>
                {
                    StatusCode = ServiceStatusCode.INVALID_REQUEST,
                    StatusMessage = StatusMessage.REQUEST_NOT_FOUND
                };
            };
            return new ServiceResponse<List<LoanAccount>>
            {

                StatusCode = ServiceStatusCode.SUCCESSFUL,
                StatusMessage = StatusMessage.SUCCESSFUl,
                ResponseObject = result
            };
        }
    }
}
