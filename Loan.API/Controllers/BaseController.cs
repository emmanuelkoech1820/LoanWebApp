using Apps.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public Account Accounts => (Account)HttpContext.Items["Account"];
        public OmniAuthUser CurrentUser
        {
            get
            {
                return new OmniAuthUser(User as ClaimsPrincipal);
            }

        }
    }

    public class OmniAuthUser : ClaimsPrincipal
    {
        public OmniAuthUser(ClaimsPrincipal principal) : base(principal)
        {

        }

        public string GetClaimValue(string key)
        {
            if (!(this.Identity is ClaimsIdentity identity))
                return null;
            var claim = this.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }

        public string Id
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.Id);
                return claim?.Value;
            }
        }

        public string CustomerId
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.CUSTOMER_ID);
                return claim?.Value;
            }
        }

        public string Username
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.USERNAME);
                return claim?.Value;
            }
        }

        public string BranchId
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.BRANCH_ID);
                return claim?.Value;
            }
        }

        public string CountryCode
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.COUNTRY_CODE);
                return claim?.Value;
            }
        }
        public string BankID
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.BANK_ID);
                return claim?.Value;
            }
        }
        public bool IsCustomer
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return false;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.CLIENT_TYPE);

                if (claim == null)
                    return false;

                return claim.Value.ToLower().Equals("customer");
            }
        }
        public bool IsServer
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return false;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.CLIENT_TYPE);

                if (claim == null)
                    return false;

                return claim.Value.ToLower().Equals("server");
            }
        }

        public string CustomerLevel
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.CUSTOMER_LEVEL);
                return claim?.Value;
            }
        }

        public string Channel
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.CHANNEL);
                return claim?.Value;
            }
        }
        public string ClientId
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.ClIENT_ID);
                return claim?.Value;
            }
        }
        public string SubChannel
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return null;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.CHANNEL);
                return claim?.Value;
            }
        }

        public bool UseTransactionLimit
        {
            get
            {
                if (!(this.Identity is ClaimsIdentity identity))
                    return false;
                var claim = this.Claims.FirstOrDefault(c => c.Type == OmniClaims.USE_TRANSACTION_LIMIT);

                if (claim == null)
                    return false;

                // Ibrahim confirm this
                // return claim.Value.ToLower().Equals(true.ToString()) && this.IsCustomer;
                return claim.Value.ToLower().Equals(true.ToString());
            }
        }

    }

    public class OmniClaims
    {
        public const string Id = "id";
        public const string CUSTOMER_ID = "customerId";
        public const string USERNAME = "username";
        public const string COUNTRY_CODE = "countryCode";
        public const string CLIENT_TYPE = "clientType";
        public const string BANK_ID = "bankId";
        public const string CHANNEL = "channel";
        public const string ClIENT_ID = "client_id";
        public const string SUB_CHANNEL = "subchannel";
        public const string USE_TRANSACTION_LIMIT = "useTransactionLimit";
        public const string CUSTOMER_LEVEL = "customerLevel";
        public const string BRANCH_ID = "branchid";
        public const string SUBJECT = "sub";
    }
}
