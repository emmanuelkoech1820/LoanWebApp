using System;

namespace Apps.Core.Models.SMSModels
{
    public class SMSRequestModel
    {
        public string Apikey { get; set; }
        public string PartnerID { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
        public string Shortcode { get; set; }
        public string Pass_type { get; set; }
    }

    public class SMSResponseModel
    {
        public Response[] responses { get; set; }
    }

    public class Response
    {
        public int Responsecode { get; set; }
        public string Responsedescription { get; set; }
        public long Mobile { get; set; }
        public string Messageid { get; set; }
        public int Networkid { get; set; }
    }



}
