using System;

namespace Apps.Core.Models.SMSModels
{
    public class SMSRequestModel
    {
        public string apikey { get; set; }
        public string partnerID { get; set; }
        public string mobile { get; set; }
        public string message { get; set; }
        public string shortcode { get; set; }
        public string pass_type { get; set; }
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
