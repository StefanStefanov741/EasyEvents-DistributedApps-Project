using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Friendship : BaseEntity
    {
        [Required]
        public int user1_id { get; set; }
        [Required]
        public int user2_id { get; set; }
        [Required]
        public DateTime befriend_date { get; set; }
        public bool pending { get; set; }
        [StringLength(50)]
        public string friendshipTier { get; set; }
    }
}
