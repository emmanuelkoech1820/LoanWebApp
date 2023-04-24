using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apps.Data.Entities
{
    public class Images : BaseModel
    {
        [ForeignKey("Accounts")]
        public string ProfileId { get; set; }
        [ForeignKey("PropertyModel")]
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}