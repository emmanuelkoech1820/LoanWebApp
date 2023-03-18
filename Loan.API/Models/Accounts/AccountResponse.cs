using Apps.Data.Entities;
using System;
using System.Collections.Generic;

namespace WebApi.Consts.Accounts
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
    }
    public class AdminDetailsResponse
    {
        public AccountResponse accountResponse { get; set; }
        public List<LoanAccount> LoanAccounts { get; set; }
        public List<Vehicle> vehicle { get; set; }
    }
}