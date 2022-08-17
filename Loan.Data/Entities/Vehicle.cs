using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class Vehicle : BaseModel
    {
        [ForeignKey("Accounts")]
        public string ProfileId { get; set; }
        public string Reference { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleType { get; set; }
        public string InsuranceCoverType { get; set; }
        public string VehicleModel { get; set; }
        public string YearOfManufacture { get; set; }

        public decimal VehicleValue { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime InsturanceStartDate { get; set; }
    }
}