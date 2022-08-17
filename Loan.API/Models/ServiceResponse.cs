using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Const;

namespace WebApi.Consts
{
    public class ServiceResponse
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public bool Successful => StatusCode == ServiceStatusCode.SUCCESSFUL;
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T ResponseObject { get; set; }
    }
    public static class ResponseCode
    {
        public static string Ok = "00";
        public static string BadRequest = "88";
        public static string Error = "99";
    }
}
