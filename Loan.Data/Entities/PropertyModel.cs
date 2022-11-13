using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class PropertyModel : BaseModel
    {
        public string Reference { get; set; }
        public string AgentId { get; set; }
        public decimal LocationId { get; set; }
        public string Price { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string Kitchens { get; set; }
        public string AdditionalInformation { get; set; }
        public bool IsEnabled { get; set; }
    }
}