using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class PropertyModel : BaseModel
    {
        public string Reference { get; set; }
        public string AgentId { get; set; }
        public int LocationId { get; set; }
        public decimal Price { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string Kitchens { get; set; }
        public string AdditionalInformation { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<Images> Images { get; set; } = new List<Images>();
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

}