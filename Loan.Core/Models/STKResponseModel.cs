using System;

namespace Apps.Core.Models.STKResponseModel
{

    public class STKResponseModel
    {
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public Additionalparameters AdditionalParameters { get; set; }
    }

    public class Additionalparameters
    {
        public string StatusDescription { get; set; }
        public string TelcoReference { get; set; }
        public string Telco { get; set; }
        public string OperationType { get; set; }
    }



}
