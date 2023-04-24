using Apps.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Core.Models
{
    public class PropertyBindingModel
    {
        public string AgentId { get; set; }
        public int LocationId { get; set; }
        public string ClientId { get; set; }
        public decimal Price { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string Kitchens { get; set; }
        public string AdditionalInformation { get; set; }

    }
    public class ImagesBindingModel: PropertyBindingModel
    {
        public string ProfileId { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public List<Images> Image { get; set; }
    }
}