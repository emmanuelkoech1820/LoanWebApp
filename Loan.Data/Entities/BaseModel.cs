using System;
using System.Collections.Generic;

namespace Apps.Data.Entities
{
    public class BaseModel
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}