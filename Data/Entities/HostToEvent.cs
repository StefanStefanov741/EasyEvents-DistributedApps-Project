using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class HostToEvent : BaseEntity
    {
        [Required]
        public int Host_id { get; set; }
        [Required]
        public int Event_id { get; set; }
    }
}
