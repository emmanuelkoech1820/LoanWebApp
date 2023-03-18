using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Const
{
    public class ServiceStatusCode
    {
        public const string SUCCESSFUL = "00";
        public const string REQUIRE_2FA = "05";
        public const string INSUFICIENT_BALANCE = "91";
        public const string INVALID_ACCOUNT_NUMBER = "92";
        public const string TRANSACTION_LIMIT_EXCEEDED = "93";
        public const string DUPLICATE_REQUEST = "94";
        public const string AUTHENTICATION_FAILED = "95";
        public const string UNABLE_TO_GET_CHARGES = "96";
        public const string INVALID_SIGNATURE = "98";
        public const string TRANSACTION_FAILED = "97";
        public const string INVALID_REQUEST = "99";

    }
    public class StatusMessage
    {
        public const string SUCCESSFUl = "Successfull";
        public const string FAILED = "Failed";
        public const string REGISTER_SUCCESS = "Registration successful, please login";
        public const string REQUEST_NOT_FOUND = "Request not found";
        public const string REGISTER_FAIL = "Failed to register, check for duplicates";
        public const string USER_NOT_FOUND = "No user exists with the email provided";
        public const string PASSWORD_RESET_FAILED = "Password reset failed";
        public const string RESET_TOKEN_INVALID = "The reset passwoed token is invalid or expired";
        public const string PASSWORD_VALIDATION_FAILED = "The account is not verified or credentials are incorrect";
        public const string CNNECTION_TEST = "connected";
        public const string EMAIL_PHONE_REQUIRED = "Email or phonenumber is required";
        public const string COULD_NOT_SEND_RESET_Token = "Could not send reset token, please try again later";
        public const string VALIDATION_RULE_BROKEN = "Stepwise validation failed";
        public const string REQUEST_VALID = "Bank transfer request valid.";
        public const string TRANSFER_FAILED = "Bank transfer Failed.";
        public const string DUPLICATE_REQUEST = "Duplicate Request";
        public const string UNAPPROVED_REQUEST = "Request not approved";
        public const string INVALID_DEST_ACCOUNT = "Invalid dest account";
        public const string VEHICLE_NOT_FOUND = "Vehicle not found";
    }
    public class BankTransferStatus
    {
        public const string PENDING_VALIDATION = "Pending Validation";
        public const string PENDING_2FA = "Pending 2FA";
        public const string PENDING_DEBIT = "Pending Debit";
        public const string PENDING_Transfer = "Pending Transfer";
        public const string SUCCESSFUL = "Successful";
        public const string FAILED = "Failed";
        public const string PENDING_VERIFICATION = "Pending Verification";
    }
    public class BankTransferAction
    {

        public const string INITIATE = "Initiate";
        public const string TRANSFER = "Transfer";
        public const string VALIDATE_REQUEST = "Validate Request";
        public const string TRANSFER_FAILED = "Transfer failed";
        public const string TRANSFER_SUCCESS = "Transfer success";

    }
}
