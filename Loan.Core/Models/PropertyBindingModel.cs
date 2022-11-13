using System;

namespace Apps.Core.Models
{
    public class PropertyBindingModel
    {
        public int AgentId { get; set; }
        public decimal LocationId { get; set; }
        public string ClientId { get; set; }
        public string Price { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string Kitchens { get; set; }
        public string AdditionalInformation { get; set; }

    }
}