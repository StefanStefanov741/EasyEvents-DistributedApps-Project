using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Like : BaseEntity
    {
        [Required]
        public int User_id { get; set; }
        [Required]
        public int Event_id { get; set; }
    }
}
